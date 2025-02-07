
namespace EmpaticaAndBioharness
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BioharnessPanel = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            BHServerRp = new System.Windows.Forms.TextBox();
            BHDisconnectBtn = new System.Windows.Forms.Button();
            BHDataPanel = new System.Windows.Forms.Panel();
            AccBHCb = new System.Windows.Forms.CheckBox();
            AccBHDataTxt = new System.Windows.Forms.TextBox();
            GeneralPackTxt = new System.Windows.Forms.TextBox();
            SummaryPackCb = new System.Windows.Forms.CheckBox();
            BatteryDataTxt = new System.Windows.Forms.TextBox();
            EcgDataTxt = new System.Windows.Forms.TextBox();
            BreathingDataTxt = new System.Windows.Forms.TextBox();
            BatteryBtn = new System.Windows.Forms.Button();
            BreathingCb = new System.Windows.Forms.CheckBox();
            EcgCb = new System.Windows.Forms.CheckBox();
            BHCnxBtn = new System.Windows.Forms.Button();
            BioharnessLbl = new System.Windows.Forms.Label();
            StartRecordingBtn = new System.Windows.Forms.Button();
            StopRecordingBtn = new System.Windows.Forms.Button();
            ExitAppBtn = new System.Windows.Forms.Button();
            RecordNotifLbl = new System.Windows.Forms.Label();
            ExitNotifLbl = new System.Windows.Forms.Label();
            RecordingRp = new System.Windows.Forms.TextBox();
            BioharnessPanel.SuspendLayout();
            BHDataPanel.SuspendLayout();
            SuspendLayout();
            // 
            // BioharnessPanel
            // 
            BioharnessPanel.Controls.Add(label2);
            BioharnessPanel.Controls.Add(BHServerRp);
            BioharnessPanel.Controls.Add(BHDisconnectBtn);
            BioharnessPanel.Controls.Add(BHDataPanel);
            BioharnessPanel.Controls.Add(BHCnxBtn);
            BioharnessPanel.Controls.Add(BioharnessLbl);
            BioharnessPanel.Location = new System.Drawing.Point(23, 14);
            BioharnessPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BioharnessPanel.Name = "BioharnessPanel";
            BioharnessPanel.Size = new System.Drawing.Size(1299, 915);
            BioharnessPanel.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(23, 665);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(180, 25);
            label2.TabIndex = 20;
            label2.Text = "Bioharness server log";
            // 
            // BHServerRp
            // 
            BHServerRp.Location = new System.Drawing.Point(9, 700);
            BHServerRp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BHServerRp.Multiline = true;
            BHServerRp.Name = "BHServerRp";
            BHServerRp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            BHServerRp.Size = new System.Drawing.Size(1267, 204);
            BHServerRp.TabIndex = 19;
            // 
            // BHDisconnectBtn
            // 
            BHDisconnectBtn.Enabled = false;
            BHDisconnectBtn.Location = new System.Drawing.Point(324, 18);
            BHDisconnectBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BHDisconnectBtn.Name = "BHDisconnectBtn";
            BHDisconnectBtn.Size = new System.Drawing.Size(144, 38);
            BHDisconnectBtn.TabIndex = 18;
            BHDisconnectBtn.Text = "Disconnect";
            BHDisconnectBtn.UseVisualStyleBackColor = true;
            BHDisconnectBtn.Click += BHDisconnectBtn_Click;
            // 
            // BHDataPanel
            // 
            BHDataPanel.Controls.Add(AccBHCb);
            BHDataPanel.Controls.Add(AccBHDataTxt);
            BHDataPanel.Controls.Add(GeneralPackTxt);
            BHDataPanel.Controls.Add(SummaryPackCb);
            BHDataPanel.Controls.Add(BatteryDataTxt);
            BHDataPanel.Controls.Add(EcgDataTxt);
            BHDataPanel.Controls.Add(BreathingDataTxt);
            BHDataPanel.Controls.Add(BatteryBtn);
            BHDataPanel.Controls.Add(BreathingCb);
            BHDataPanel.Controls.Add(EcgCb);
            BHDataPanel.Enabled = false;
            BHDataPanel.Location = new System.Drawing.Point(9, 67);
            BHDataPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BHDataPanel.Name = "BHDataPanel";
            BHDataPanel.Size = new System.Drawing.Size(1286, 581);
            BHDataPanel.TabIndex = 17;
            // 
            // AccBHCb
            // 
            AccBHCb.AutoSize = true;
            AccBHCb.Enabled = false;
            AccBHCb.Location = new System.Drawing.Point(14, 260);
            AccBHCb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            AccBHCb.Name = "AccBHCb";
            AccBHCb.Size = new System.Drawing.Size(72, 29);
            AccBHCb.TabIndex = 21;
            AccBHCb.Text = "ACC";
            AccBHCb.UseVisualStyleBackColor = true;
            // 
            // AccBHDataTxt
            // 
            AccBHDataTxt.Enabled = false;
            AccBHDataTxt.Location = new System.Drawing.Point(133, 257);
            AccBHDataTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            AccBHDataTxt.Multiline = true;
            AccBHDataTxt.Name = "AccBHDataTxt";
            AccBHDataTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            AccBHDataTxt.Size = new System.Drawing.Size(633, 99);
            AccBHDataTxt.TabIndex = 20;
            AccBHDataTxt.Text = "No config";
            // 
            // GeneralPackTxt
            // 
            GeneralPackTxt.Location = new System.Drawing.Point(790, 65);
            GeneralPackTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            GeneralPackTxt.Multiline = true;
            GeneralPackTxt.Name = "GeneralPackTxt";
            GeneralPackTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            GeneralPackTxt.Size = new System.Drawing.Size(477, 504);
            GeneralPackTxt.TabIndex = 19;
            // 
            // SummaryPackCb
            // 
            SummaryPackCb.AutoSize = true;
            SummaryPackCb.Location = new System.Drawing.Point(811, 23);
            SummaryPackCb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            SummaryPackCb.Name = "SummaryPackCb";
            SummaryPackCb.Size = new System.Drawing.Size(156, 29);
            SummaryPackCb.TabIndex = 18;
            SummaryPackCb.Text = "Summary pack";
            SummaryPackCb.UseVisualStyleBackColor = true;
            SummaryPackCb.CheckedChanged += SummaryPackCb_CheckedChanged;
            // 
            // BatteryDataTxt
            // 
            BatteryDataTxt.Location = new System.Drawing.Point(133, 377);
            BatteryDataTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BatteryDataTxt.Multiline = true;
            BatteryDataTxt.Name = "BatteryDataTxt";
            BatteryDataTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            BatteryDataTxt.Size = new System.Drawing.Size(633, 102);
            BatteryDataTxt.TabIndex = 17;
            // 
            // EcgDataTxt
            // 
            EcgDataTxt.Location = new System.Drawing.Point(133, 135);
            EcgDataTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            EcgDataTxt.Multiline = true;
            EcgDataTxt.Name = "EcgDataTxt";
            EcgDataTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            EcgDataTxt.Size = new System.Drawing.Size(633, 99);
            EcgDataTxt.TabIndex = 16;
            // 
            // BreathingDataTxt
            // 
            BreathingDataTxt.Location = new System.Drawing.Point(133, 15);
            BreathingDataTxt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BreathingDataTxt.Multiline = true;
            BreathingDataTxt.Name = "BreathingDataTxt";
            BreathingDataTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            BreathingDataTxt.Size = new System.Drawing.Size(633, 101);
            BreathingDataTxt.TabIndex = 15;
            // 
            // BatteryBtn
            // 
            BatteryBtn.Location = new System.Drawing.Point(14, 377);
            BatteryBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BatteryBtn.Name = "BatteryBtn";
            BatteryBtn.Size = new System.Drawing.Size(107, 38);
            BatteryBtn.TabIndex = 14;
            BatteryBtn.Text = "Battery";
            BatteryBtn.UseVisualStyleBackColor = true;
            BatteryBtn.Click += BatteryBtn_Click;
            // 
            // BreathingCb
            // 
            BreathingCb.AutoSize = true;
            BreathingCb.Location = new System.Drawing.Point(14, 23);
            BreathingCb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BreathingCb.Name = "BreathingCb";
            BreathingCb.Size = new System.Drawing.Size(69, 29);
            BreathingCb.TabIndex = 10;
            BreathingCb.Text = "RSP";
            BreathingCb.UseVisualStyleBackColor = true;
            BreathingCb.CheckedChanged += BreathingCb_CheckedChanged;
            // 
            // EcgCb
            // 
            EcgCb.AutoSize = true;
            EcgCb.Location = new System.Drawing.Point(14, 138);
            EcgCb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            EcgCb.Name = "EcgCb";
            EcgCb.Size = new System.Drawing.Size(70, 29);
            EcgCb.TabIndex = 11;
            EcgCb.Text = "ECG";
            EcgCb.UseVisualStyleBackColor = true;
            EcgCb.CheckedChanged += EcgCb_CheckedChanged;
            // 
            // BHCnxBtn
            // 
            BHCnxBtn.Location = new System.Drawing.Point(209, 18);
            BHCnxBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            BHCnxBtn.Name = "BHCnxBtn";
            BHCnxBtn.Size = new System.Drawing.Size(107, 38);
            BHCnxBtn.TabIndex = 12;
            BHCnxBtn.Text = "Connect";
            BHCnxBtn.UseVisualStyleBackColor = true;
            BHCnxBtn.Click += BHCnxBtn_Click;
            // 
            // BioharnessLbl
            // 
            BioharnessLbl.AutoSize = true;
            BioharnessLbl.Location = new System.Drawing.Point(23, 25);
            BioharnessLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            BioharnessLbl.Name = "BioharnessLbl";
            BioharnessLbl.Size = new System.Drawing.Size(97, 25);
            BioharnessLbl.TabIndex = 3;
            BioharnessLbl.Text = "Bioharness";
            // 
            // StartRecordingBtn
            // 
            StartRecordingBtn.Location = new System.Drawing.Point(46, 987);
            StartRecordingBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            StartRecordingBtn.Name = "StartRecordingBtn";
            StartRecordingBtn.Size = new System.Drawing.Size(170, 38);
            StartRecordingBtn.TabIndex = 2;
            StartRecordingBtn.Text = "Start recording";
            StartRecordingBtn.UseVisualStyleBackColor = true;
            StartRecordingBtn.Click += RecordingBtn_Click;
            // 
            // StopRecordingBtn
            // 
            StopRecordingBtn.Enabled = false;
            StopRecordingBtn.Location = new System.Drawing.Point(254, 987);
            StopRecordingBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            StopRecordingBtn.Name = "StopRecordingBtn";
            StopRecordingBtn.Size = new System.Drawing.Size(170, 38);
            StopRecordingBtn.TabIndex = 3;
            StopRecordingBtn.Text = "Stop recording";
            StopRecordingBtn.UseVisualStyleBackColor = true;
            StopRecordingBtn.Click += StopRecordingBtn_Click;
            // 
            // ExitAppBtn
            // 
            ExitAppBtn.Location = new System.Drawing.Point(1190, 987);
            ExitAppBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            ExitAppBtn.Name = "ExitAppBtn";
            ExitAppBtn.Size = new System.Drawing.Size(109, 38);
            ExitAppBtn.TabIndex = 4;
            ExitAppBtn.Text = "Exit";
            ExitAppBtn.UseVisualStyleBackColor = true;
            ExitAppBtn.Click += ExitAppBtn_Click;
            // 
            // RecordNotifLbl
            // 
            RecordNotifLbl.AutoSize = true;
            RecordNotifLbl.ForeColor = System.Drawing.Color.Red;
            RecordNotifLbl.Location = new System.Drawing.Point(49, 941);
            RecordNotifLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            RecordNotifLbl.Name = "RecordNotifLbl";
            RecordNotifLbl.Size = new System.Drawing.Size(0, 25);
            RecordNotifLbl.TabIndex = 5;
            // 
            // ExitNotifLbl
            // 
            ExitNotifLbl.AutoSize = true;
            ExitNotifLbl.ForeColor = System.Drawing.Color.Red;
            ExitNotifLbl.Location = new System.Drawing.Point(1176, 941);
            ExitNotifLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            ExitNotifLbl.Name = "ExitNotifLbl";
            ExitNotifLbl.Size = new System.Drawing.Size(0, 25);
            ExitNotifLbl.TabIndex = 6;
            // 
            // RecordingRp
            // 
            RecordingRp.Location = new System.Drawing.Point(23, 1035);
            RecordingRp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            RecordingRp.Multiline = true;
            RecordingRp.Name = "RecordingRp";
            RecordingRp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            RecordingRp.Size = new System.Drawing.Size(444, 114);
            RecordingRp.TabIndex = 21;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1343, 1164);
            ControlBox = false;
            Controls.Add(RecordingRp);
            Controls.Add(ExitNotifLbl);
            Controls.Add(RecordNotifLbl);
            Controls.Add(ExitAppBtn);
            Controls.Add(StopRecordingBtn);
            Controls.Add(StartRecordingBtn);
            Controls.Add(BioharnessPanel);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "MainForm";
            Text = "Main";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormClosed += MainForm_FormClosed;
            BioharnessPanel.ResumeLayout(false);
            BioharnessPanel.PerformLayout();
            BHDataPanel.ResumeLayout(false);
            BHDataPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Panel BioharnessPanel;
        private System.Windows.Forms.Label BioharnessLbl;
        private System.Windows.Forms.Button BatteryBtn;
        private System.Windows.Forms.CheckBox BreathingCb;
        private System.Windows.Forms.CheckBox EcgCb;
        private System.Windows.Forms.TextBox EcgDataTxt;
        private System.Windows.Forms.TextBox BreathingDataTxt;
        private System.Windows.Forms.Button BHCnxBtn;
        private System.Windows.Forms.Panel BHDataPanel;
        private System.Windows.Forms.Button StartRecordingBtn;
        private System.Windows.Forms.Button BHDisconnectBtn;
        private System.Windows.Forms.TextBox BatteryDataTxt;
        private System.Windows.Forms.Button StopRecordingBtn;
        private System.Windows.Forms.Button ExitAppBtn;
        private System.Windows.Forms.Label RecordNotifLbl;
        private System.Windows.Forms.TextBox BHServerRp;
        private System.Windows.Forms.Label ExitNotifLbl;
        private System.Windows.Forms.TextBox GeneralPackTxt;
        private System.Windows.Forms.CheckBox SummaryPackCb;
        private System.Windows.Forms.CheckBox AccBHCb;
        private System.Windows.Forms.TextBox AccBHDataTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RecordingRp;
    }
}

