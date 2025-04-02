namespace BluetoothBatteryMonitor
{
    partial class SettingsForm
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
        private void InitializeComponent() {
            label1 = new Label();
            WarningLevelUpDown = new NumericUpDown();
            SaveButton = new Button();
            DismissButton = new Button();
            label2 = new Label();
            DeviceNameComboBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)WarningLevelUpDown).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 15);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 0;
            label1.Text = "Device Name";
            // 
            // WarningLevelUpDown
            // 
            WarningLevelUpDown.Location = new Point(130, 41);
            WarningLevelUpDown.Name = "WarningLevelUpDown";
            WarningLevelUpDown.Size = new Size(113, 23);
            WarningLevelUpDown.TabIndex = 2;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SaveButton.Location = new Point(182, 88);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 3;
            SaveButton.Text = "OK";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // DismissButton
            // 
            DismissButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            DismissButton.Location = new Point(263, 88);
            DismissButton.Name = "DismissButton";
            DismissButton.Size = new Size(75, 23);
            DismissButton.TabIndex = 4;
            DismissButton.Text = "Cancel";
            DismissButton.UseVisualStyleBackColor = true;
            DismissButton.Click += DismissButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 43);
            label2.Name = "label2";
            label2.Size = new Size(103, 15);
            label2.TabIndex = 5;
            label2.Text = "Warning Level (%)";
            // 
            // DeviceNameComboBox
            // 
            DeviceNameComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DeviceNameComboBox.DropDownWidth = 300;
            DeviceNameComboBox.FormattingEnabled = true;
            DeviceNameComboBox.Location = new Point(130, 12);
            DeviceNameComboBox.Name = "DeviceNameComboBox";
            DeviceNameComboBox.Size = new Size(208, 23);
            DeviceNameComboBox.TabIndex = 6;
            // 
            // SettingsForm
            // 
            AcceptButton = SaveButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = DismissButton;
            ClientSize = new Size(350, 123);
            Controls.Add(DeviceNameComboBox);
            Controls.Add(label2);
            Controls.Add(DismissButton);
            Controls.Add(SaveButton);
            Controls.Add(WarningLevelUpDown);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bluetooth Battery Monitor Settings";
            ((System.ComponentModel.ISupportInitialize)WarningLevelUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown WarningLevelUpDown;
        private Button SaveButton;
        private Button DismissButton;
        private Label label2;
        private ComboBox DeviceNameComboBox;
    }
}
