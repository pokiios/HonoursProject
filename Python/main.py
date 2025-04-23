# Load NeuroKit and other useful packages

import os
import neurokit2 as nk
import pandas as pd
import numpy as np
import time
import keyboard
import threading

should_exit = False

# Loads file from path and filename
def LoadFile(path, filename):
    try:
        raw_df = pd.read_csv(os.path.join(path, filename), header=None) # Read raw dataframe
        if len(raw_df.columns) >= 2: # Check if the dataframe has at least 2 columns
            df = raw_df.iloc[:, 0:2]  # Select first two columns
        else:
            df = raw_df
        return df
    except Exception as e:  # Catch the actual error
        print(f'Error ECG file load: {e}')
        return None

# Segments the data based on start and end time
def SegmentData(df, start_time, end_time=None):
    try:
        if len(df) > 2:
            if start_time > 0:
                df = df.loc[df[1] > start_time]
            else:
                # set start_session value to the mean value of the data portion 
                df[0] = pd.to_numeric(df[0], errors="coerce")  # Convert strings to NaN if they are non-numeric
                df.loc[0,0] = df[1::len(df)-1][0].mean()
                
            if (end_time != None) and (end_time < df.loc[df.index[-1], 1]):
                # delete rows where dataTime > sessionEndTime (remove the data recorder after end of session (buffer leftovers before the end of the thread))
                df = df.loc[df[1] < end_time]
            else:
                df = df.loc[df[1] < df.loc[df.index[-1], 1]]
            
            df = df.reset_index(drop=True) # Reset index after filtering
            
            # Converts data to float
            df[0] = df[0].astype(float) 

            return df
        else:
            print("DataFrame is too small to segment.")
            return None
        
    except Exception as e:
        print(f"Error segmenting data, returning None: {e}")
        return None


# Getting Signal from ECG
def GetECGSignal(df, ecg_sampling_rate):
    # Check if dataframe is empty
    if df is None:
        return None
    try:
        signal, _ = nk.ecg_process(df[0], sampling_rate = ecg_sampling_rate)
        return signal
    except Exception as e:
        print(f"Error processing ECG Signal, returning None. {e}")
        return None

# Getting Peaks from processed ECG Signal
def GetECGPeaks(signal):
    if signal is None:
        return None
    return signal["ECG_R_Peaks"]

# Getting The RMSSD from the Peaks (Smaller number means higher activity)
def GetECGRMSSD(peaks, ecg_sampling_rate):
    if peaks is None:
        return None
    try:
        hrv_out = nk.hrv_time(peaks, sampling_rate = ecg_sampling_rate)
        return hrv_out['HRV_RMSSD'].values[0]
    except Exception as e:
        print(f"Error Extracting ECG Features, Returning None. {e}")
        return None

# Geting the RSP Signal
def GetRSPSignal(df, rsp_sampling_rate):
    # Check if the DataFrame is None or empty
    if df is None:
        return None
    try:
        signal, _ = nk.rsp_process(df[0], sampling_rate = rsp_sampling_rate)
        return signal
    except Exception as e:
        print(f"Error processing RSP Signal, returning None. {e}")
        return None

    
def GetRSPRate(signal):
    # Check if the signal is None or empty
    if signal is None:
        return None
    
    # Check if RSP_Rate exists in the signal DataFrame
    try:
        rate = signal["RSP_Rate"]
        return rate if pd.isna(rate) else 0 
    except Exception as e:
        print(f"Error processing RSP Rate: {e}")
        return 0

def KeyMonitor():
    global should_exit # Global flag for quitting program
    if keyboard.read_key() == 'q':
        should_exit = True
        print("Exit key has been pressed, closing program...")

if __name__ == "__main__":
    path = (r"D:\_School\HonoursProject\Python\bioharness\bin\Debug\netcoreapp3.1\Experiment\Session") # Raw Filepath
    ecg_filename = "ecgLog.csv"
    rsp_filename = "breathingLog.csv"
    ecg_sampling_rate = 250 # Samples per Second (Sourced from BioHarness 3.0 Documentation)
    rsp_sampling_rate = 18 # Samples per Second (Sourced from BioHarness 3.0 Documentation)
    reading_times = 30 # Seconds between readings
    timer = 0
    print("Welcome to the data processing application!\n----------------------------------------")
    print("Press q to Exit program.")
    
    data = {
    "Timestamp":[], 
    "RMSSD":   [], 
    "RSP":  []
    }
    
    keyboard_thread = threading.Thread(target=KeyMonitor)  # Create a thread for keyboard monitoring
    keyboard_thread.daemon = True  # once the main function has quit, the thread will quit too
    keyboard_thread.start()
    
    # Dataframe from dictionary
    collected_data = pd.DataFrame.from_dict(data)
    
    # CSV from Dataframe
    collected_data.to_csv(
    r"D:\_School\HonoursProject\Assets\CSV\CollectedData.csv",
    mode='w',  # Overwrite
    index=False, 
    header=True  
    )

    # Main loop
    while not should_exit:
        # One second counter
        print(timer)
        timer += 1
        time.sleep(1)  
        
        # check if timer has reached time passed 
        if timer % reading_times == 0:
            
            # Get start and end readings 
            data_segment_start_time = timer - reading_times
            data_segment_end_time = timer
            
            # Load and parse ecg data
            ecg_df = LoadFile(path, ecg_filename)
            ecg_df = SegmentData(ecg_df, data_segment_start_time, data_segment_end_time)

            # Calculates the RMSSD from the ECG signal
            ecg_signal = GetECGSignal(ecg_df, ecg_sampling_rate)
            ecg_peaks = GetECGPeaks(ecg_signal)
            rmssd = GetECGRMSSD(ecg_peaks, ecg_sampling_rate)

            # Load and parse rsp data
            rsp_df = LoadFile(path, rsp_filename)
            rsp_df = SegmentData(rsp_df, data_segment_start_time, data_segment_end_time)
                
            # Calculates the RSP rate from the RSP signal
            rsp_signal = GetRSPRate(rsp_df, rsp_sampling_rate)
            rsp_rate = GetRSPSignal(rsp_signal)
            
            # Creates a new row with the current timestamp and the calculated RMSSD and RSP values
            new_row = { 
                'Timestamp': [timer],
                'RMSSD': [rmssd if rmssd is not None else 0],  # Ensure 0 if None
                'RSP': [rsp_rate if rsp_rate is not None else 0]  # Ensure 0 if None
            }
            
            # Turn Dictionary into DataFrame
            new_row_df = pd.DataFrame(new_row)  # Create a DataFrame from the new row

            # Append new row to the existing CSV
            new_row_df.to_csv(r"D:\_School\HonoursProject\Assets\CSV\CollectedData.csv",
                      mode='a',  # append
                      header=False,  # Do not write header again
                      index=False)  # Remove default index column
            
    print("Terminated program successfully.")