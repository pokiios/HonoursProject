# Load NeuroKit and other useful packages

import os
from signal import getsignal
import neurokit2 as nk
import pandas as pd
import numpy as np
import time
import warnings
import cv

# Loads file from path and filename
def LoadFile(path, filename):
    try:
        df = pd.read_csv(os.path.join(path, filename), header = None, usecols=[0,1])
        return df
    except:
        print('Error ECG file load')
        return None

def SegmentData(df, start_time, end_time=None):
    if len(df) > 2:
        if start_time > 0:
            df = df.loc[df[1] > start_time]
        else:
            # set start_session value to the mean value of the data portion 
            df.loc[0,0] = df[1::len(df)-1][0].mean()
        if (end_time != None) and (end_time < df.loc[df.index[-1], 1]):
            # delete rows where dataTime > sessionEndTime (remove the data recorder after end of session (buffer leftovers before the end of the thread))
            df.loc[df[1] < end_time]
        else:
            df = df.loc[df[1] < df.loc[df.index[-1], 1]]
        
        df = df.reset_index(drop=True)
        
        # Converts data to float
        df[0] = df[0].astype(float)
        
        return df
    else:
        print("Empty Dataframe, returning none")
        return None

# Getting Signal from ECG
def GetECGSignal(df, sampling_rate):
    if df is None:
        return None
    try:
        signal, _ = nk.ecg_process(df[0], sampling_rate = sampling_rate)
        return signal
    except:
        print("Error processing ECG Signal, returning None")
        return None

# Getting Peaks from processed ECG Signal
def GetPeaks(signal):
    if signal == None:
        return None
    return signal["ECG_R_Peaks"]

# Getting The RMSSD from the Peaks (Smaller number means higher activity)
def GetRMSSD(peaks, sampling_rate):
    if peaks is None:
        return None
    try:
        hrv_out = nk.hrv_time(peaks, sampling_rate = sampling_rate)
        return hrv_out['HRV_RMSSD']
    except:
        print("Error Extracting ECG Features, Returning None.")
        return None

# Geting the RSP Signal
def GetRSPSignal(df, sampling_rate):
    if df is None:
        return None
    try:
        signal, _ = nk.rsp_process(df[0], sampling_rate = sampling_rate)
        return signal
    except:
        print("Error processing RSP Signal, returning None")
        return None
    
def GetRate(signal):
    if signal is None:
        return None
    return signal["RSP_Rate"]

if __name__ == "__main__":
    warnings.filterwarnings("ignore")
 
    path = ".\bioharness\bin\Debug\netcoreapp3.1\Experiment\Session"
    ecg_filename = "ecgLog.csv"
    rsp_filename = "breathingLog.csv"
    ecg_sampling_rate = 250
    rsp_sampling_rate = 18
    timer = 0
    reading_times = 5
    exitKey = cv.WaitKey(7) % 0x100
    
    print("Press ESC or Enter to Exit program.")

    while True:

        time.sleep(1)
        timer += 1
        if timer % reading_times == 0:
            data_segment_start_time = timer - reading_times
            data_segment_end_time = timer
            ecg_df = LoadFile(path, ecg_filename)
            ecg_df = SegmentData(ecg_df, data_segment_start_time, data_segment_end_time)
            ecg_signal = GetECGSignal(ecg_df, ecg_sampling_rate)
            ecg_peaks = GetPeaks(ecg_signal)
            rmssd = GetRMSSD(ecg_peaks, ecg_sampling_rate)

            rsp_df = LoadFile(path, rsp_filename)
            rsp_df = SegmentData(rsp_df, data_segment_start_time, data_segment_end_time)
            rsp_signal = GetRSPSignal(rsp_df, rsp_sampling_rate)
            rsp_rate = GetRate(rsp_signal)
            
            #export dataframes
            ecg_df.df_to_csv("../output/ecg_df.csv")
            rsp_df.df_to_csv("../output/rsp_df.csv")

            

        if exitKey == 27 or exitKey == 10: # if press escape or enter
            break