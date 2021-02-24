namespace TestSlots
{
    partial class TestSlotsDll
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Spins_TextBox = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Bet_DropBox = new System.Windows.Forms.ComboBox();
            this.Denom_DropBox = new System.Windows.Forms.ComboBox();
            this.CSharpRandom_RadioButton = new System.Windows.Forms.RadioButton();
            this.IGamingRandom_RadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Bet_Label = new System.Windows.Forms.Label();
            this.Denom_Label = new System.Windows.Forms.Label();
            this.Spins_Label = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Status_Label = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.FreeSpins_RadioButton = new System.Windows.Forms.RadioButton();
            this.RegularSpins_RadioButton = new System.Windows.Forms.RadioButton();
            this.Mix_RadioButton = new System.Windows.Forms.RadioButton();
            this.FSTotalAmount_DropBox = new System.Windows.Forms.ComboBox();
            this.FNFNSpin = new System.Windows.Forms.Button();
            this.RunButton = new System.Windows.Forms.Button();
            this.PrintTable_Button = new System.Windows.Forms.Button();
            this.NonRandom_RadioButton = new System.Windows.Forms.RadioButton();
            this.Random_RadioButton = new System.Windows.Forms.RadioButton();
            this.SimulateMaxWin_Button = new System.Windows.Forms.Button();
            this.FSTotalAmount_Label = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Spins_TextBox
            // 
            this.Spins_TextBox.Location = new System.Drawing.Point(441, 99);
            this.Spins_TextBox.Name = "Spins_TextBox";
            this.Spins_TextBox.Size = new System.Drawing.Size(100, 20);
            this.Spins_TextBox.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(10, 126);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(801, 261);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // Bet_DropBox
            // 
            this.Bet_DropBox.FormattingEnabled = true;
            this.Bet_DropBox.Items.AddRange(new object[] {
                "88", "176", "264", "528", "880"});
            this.Bet_DropBox.Location = new System.Drawing.Point(244, 99);
            this.Bet_DropBox.Name = "Bet_DropBox";
            this.Bet_DropBox.Size = new System.Drawing.Size(121, 21);
            this.Bet_DropBox.TabIndex = 4;
            this.Bet_DropBox.SelectedIndexChanged += new System.EventHandler(this.Bet_DropBox_SelectedIndexChanged);
            // 
            // Denom_DropBox
            // 
            this.Denom_DropBox.FormattingEnabled = true;
            this.Denom_DropBox.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "20"});
            this.Denom_DropBox.Location = new System.Drawing.Point(371, 99);
            this.Denom_DropBox.Name = "Denom_DropBox";
            this.Denom_DropBox.Size = new System.Drawing.Size(55, 21);
            this.Denom_DropBox.TabIndex = 5;
            // 
            // CSharpRandom_RadioButton
            // 
            this.CSharpRandom_RadioButton.AutoSize = true;
            this.CSharpRandom_RadioButton.Checked = true;
            this.CSharpRandom_RadioButton.Location = new System.Drawing.Point(6, 19);
            this.CSharpRandom_RadioButton.Name = "CSharpRandom_RadioButton";
            this.CSharpRandom_RadioButton.Size = new System.Drawing.Size(82, 17);
            this.CSharpRandom_RadioButton.TabIndex = 6;
            this.CSharpRandom_RadioButton.TabStop = true;
            this.CSharpRandom_RadioButton.Text = "C# Random";
            this.CSharpRandom_RadioButton.UseVisualStyleBackColor = true;
            // 
            // IGamingRandom_RadioButton
            // 
            this.IGamingRandom_RadioButton.AutoSize = true;
            this.IGamingRandom_RadioButton.Location = new System.Drawing.Point(93, 19);
            this.IGamingRandom_RadioButton.Name = "IGamingRandom_RadioButton";
            this.IGamingRandom_RadioButton.Size = new System.Drawing.Size(107, 17);
            this.IGamingRandom_RadioButton.TabIndex = 7;
            this.IGamingRandom_RadioButton.TabStop = true;
            this.IGamingRandom_RadioButton.Text = "IGaming Random";
            this.IGamingRandom_RadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.IGamingRandom_RadioButton);
            this.groupBox1.Controls.Add(this.CSharpRandom_RadioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 45);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Randomizer";
            // 
            // Bet_Label
            // 
            this.Bet_Label.AutoSize = true;
            this.Bet_Label.Location = new System.Drawing.Point(241, 80);
            this.Bet_Label.Name = "Bet_Label";
            this.Bet_Label.Size = new System.Drawing.Size(23, 13);
            this.Bet_Label.TabIndex = 9;
            this.Bet_Label.Text = "Bet";
            // 
            // Denom_Label
            // 
            this.Denom_Label.AutoSize = true;
            this.Denom_Label.Location = new System.Drawing.Point(368, 80);
            this.Denom_Label.Name = "Denom_Label";
            this.Denom_Label.Size = new System.Drawing.Size(41, 13);
            this.Denom_Label.TabIndex = 10;
            this.Denom_Label.Text = "Denom";
            // 
            // Spins_Label
            // 
            this.Spins_Label.AutoSize = true;
            this.Spins_Label.Location = new System.Drawing.Point(438, 80);
            this.Spins_Label.Name = "Spins_Label";
            this.Spins_Label.Size = new System.Drawing.Size(33, 13);
            this.Spins_Label.TabIndex = 11;
            this.Spins_Label.Text = "Spins";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 393);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(798, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // Status_Label
            // 
            this.Status_Label.AutoSize = true;
            this.Status_Label.Location = new System.Drawing.Point(12, 422);
            this.Status_Label.Name = "Status_Label";
            this.Status_Label.Size = new System.Drawing.Size(35, 13);
            this.Status_Label.TabIndex = 14;
            this.Status_Label.Text = "status";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.FreeSpins_RadioButton);
            this.groupBox2.Controls.Add(this.RegularSpins_RadioButton);
            this.groupBox2.Controls.Add(this.Mix_RadioButton);
            this.groupBox2.Location = new System.Drawing.Point(10, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(355, 37);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Simulator Type";
            // 
            // FreeSpins_RadioButton
            // 
            this.FreeSpins_RadioButton.AutoSize = true;
            this.FreeSpins_RadioButton.Location = new System.Drawing.Point(170, 14);
            this.FreeSpins_RadioButton.Name = "FreeSpins_RadioButton";
            this.FreeSpins_RadioButton.Size = new System.Drawing.Size(72, 17);
            this.FreeSpins_RadioButton.TabIndex = 2;
            this.FreeSpins_RadioButton.Text = "FreeSpins";
            this.FreeSpins_RadioButton.UseVisualStyleBackColor = true;
            // 
            // RegularSpins_RadioButton
            // 
            this.RegularSpins_RadioButton.AutoSize = true;
            this.RegularSpins_RadioButton.Location = new System.Drawing.Point(76, 14);
            this.RegularSpins_RadioButton.Name = "RegularSpins_RadioButton";
            this.RegularSpins_RadioButton.Size = new System.Drawing.Size(88, 17);
            this.RegularSpins_RadioButton.TabIndex = 1;
            this.RegularSpins_RadioButton.Text = "RegularSpins";
            this.RegularSpins_RadioButton.UseVisualStyleBackColor = true;
            // 
            // Mix_RadioButton
            // 
            this.Mix_RadioButton.AutoSize = true;
            this.Mix_RadioButton.Checked = true;
            this.Mix_RadioButton.Location = new System.Drawing.Point(8, 14);
            this.Mix_RadioButton.Name = "Mix_RadioButton";
            this.Mix_RadioButton.Size = new System.Drawing.Size(41, 17);
            this.Mix_RadioButton.TabIndex = 0;
            this.Mix_RadioButton.TabStop = true;
            this.Mix_RadioButton.Text = "Mix";
            this.Mix_RadioButton.UseVisualStyleBackColor = true;
            // 
            // FSTotalAmount_DropBox
            // 
            this.FSTotalAmount_DropBox.FormattingEnabled = true;
            this.FSTotalAmount_DropBox.Items.AddRange(new object[] {
            "5",
            "10",
            "15"});
            this.FSTotalAmount_DropBox.Location = new System.Drawing.Point(547, 99);
            this.FSTotalAmount_DropBox.Name = "FSTotalAmount_DropBox";
            this.FSTotalAmount_DropBox.Size = new System.Drawing.Size(96, 21);
            this.FSTotalAmount_DropBox.TabIndex = 18;
            // 
            // FNFNSpin
            // 
            this.FNFNSpin.Location = new System.Drawing.Point(671, 96);
            this.FNFNSpin.Name = "FNFNSpin";
            this.FNFNSpin.Size = new System.Drawing.Size(75, 23);
            this.FNFNSpin.TabIndex = 21;
            this.FNFNSpin.Text = "FNFNSpin";
            this.FNFNSpin.UseVisualStyleBackColor = true;
            this.FNFNSpin.Click += new System.EventHandler(this.FNFNSpin_Click);
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(752, 96);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(75, 23);
            this.RunButton.TabIndex = 26;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // PrintTable_Button
            // 
            this.PrintTable_Button.Location = new System.Drawing.Point(736, 26);
            this.PrintTable_Button.Name = "PrintTable_Button";
            this.PrintTable_Button.Size = new System.Drawing.Size(75, 23);
            this.PrintTable_Button.TabIndex = 27;
            this.PrintTable_Button.Text = "Print Table";
            this.PrintTable_Button.UseVisualStyleBackColor = true;
            this.PrintTable_Button.Click += new System.EventHandler(this.PrintTable_Button_Click);
            // 
            // NonRandom_RadioButton
            // 

            this.NonRandom_RadioButton.AutoSize = true;
            this.NonRandom_RadioButton.Location = new System.Drawing.Point(645, 26);
            this.NonRandom_RadioButton.Name = "NonRandom_RadioButton";
            this.NonRandom_RadioButton.Size = new System.Drawing.Size(85, 17);
            this.NonRandom_RadioButton.TabIndex = 28;
            this.NonRandom_RadioButton.Text = "NonRandom";
            this.NonRandom_RadioButton.UseVisualStyleBackColor = true;
            // 
            // SimulateMaxWin_Button
            // 
            this.SimulateMaxWin_Button.Location = new System.Drawing.Point(406, 23);
            this.SimulateMaxWin_Button.Name = "SimulateMaxWin_Button";
            this.SimulateMaxWin_Button.Size = new System.Drawing.Size(105, 23);
            this.SimulateMaxWin_Button.TabIndex = 30;
            this.SimulateMaxWin_Button.Text = "Simulate Max Win";
            this.SimulateMaxWin_Button.UseVisualStyleBackColor = true;
            this.SimulateMaxWin_Button.Click += new System.EventHandler(this.button1_Click);
            // 
            // Random_RadioButton
            // 
            this.Random_RadioButton.AutoSize = true;
            this.Random_RadioButton.Checked = true;
            this.Random_RadioButton.Location = new System.Drawing.Point(554, 26);
            this.Random_RadioButton.Name = "Random_RadioButton";
            this.Random_RadioButton.Size = new System.Drawing.Size(65, 17);
            this.Random_RadioButton.TabIndex = 29;
            this.Random_RadioButton.TabStop = true;
            this.Random_RadioButton.Text = "Random";
            this.Random_RadioButton.UseVisualStyleBackColor = true;
            // 
            // FSTotalAmount_Label
            // 
            this.FSTotalAmount_Label.AutoSize = true;
            this.FSTotalAmount_Label.Location = new System.Drawing.Point(544, 80);
            this.FSTotalAmount_Label.Name = "FSTotalAmount_Label";
            this.FSTotalAmount_Label.Size = new System.Drawing.Size(80, 13);
            this.FSTotalAmount_Label.TabIndex = 31;
            this.FSTotalAmount_Label.Text = "FSTotalAmount";
            // 
            // TestSlotsDll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 484);
            this.Controls.Add(this.FSTotalAmount_Label);
            this.Controls.Add(this.SimulateMaxWin_Button);
            this.Controls.Add(this.Random_RadioButton);
            this.Controls.Add(this.NonRandom_RadioButton);
            this.Controls.Add(this.PrintTable_Button);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.FNFNSpin);
            this.Controls.Add(this.FSTotalAmount_DropBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Status_Label);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Spins_Label);
            this.Controls.Add(this.Denom_Label);
            this.Controls.Add(this.Bet_Label);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Denom_DropBox);
            this.Controls.Add(this.Bet_DropBox);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Spins_TextBox);
            this.Name = "TestSlotsDll";
            this.Text = "TestSlotsDll";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox Spins_TextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox Bet_DropBox;
        private System.Windows.Forms.ComboBox Denom_DropBox;
        private System.Windows.Forms.RadioButton CSharpRandom_RadioButton;
        private System.Windows.Forms.RadioButton IGamingRandom_RadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Bet_Label;
        private System.Windows.Forms.Label Denom_Label;
        private System.Windows.Forms.Label Spins_Label;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label Status_Label;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton FreeSpins_RadioButton;
        private System.Windows.Forms.RadioButton RegularSpins_RadioButton;
        private System.Windows.Forms.RadioButton Mix_RadioButton;
        private System.Windows.Forms.ComboBox FSTotalAmount_DropBox;
        private System.Windows.Forms.Button FNFNSpin;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.Button PrintTable_Button;
        private System.Windows.Forms.RadioButton NonRandom_RadioButton;
        private System.Windows.Forms.RadioButton Random_RadioButton;
        private System.Windows.Forms.Button SimulateMaxWin_Button;
        private System.Windows.Forms.Label FSTotalAmount_Label;
    }
}