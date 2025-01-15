namespace PROTATool
{
    partial class PROTATool
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPR = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIPs = new System.Windows.Forms.TextBox();
            this.checkBoxFlashOnChange = new System.Windows.Forms.CheckBox();
            this.labelState = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Pull request link from Github:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxPR
            // 
            this.textBoxPR.Location = new System.Drawing.Point(17, 119);
            this.textBoxPR.Name = "textBoxPR";
            this.textBoxPR.Size = new System.Drawing.Size(504, 20);
            this.textBoxPR.TabIndex = 5;
            this.textBoxPR.Text = "https://github.com/openshwprojects/OpenBK7231T_App/pull/1464";
            this.textBoxPR.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Target device IPs; use ; to separate";
            // 
            // textBoxIPs
            // 
            this.textBoxIPs.Location = new System.Drawing.Point(17, 74);
            this.textBoxIPs.Name = "textBoxIPs";
            this.textBoxIPs.Size = new System.Drawing.Size(504, 20);
            this.textBoxIPs.TabIndex = 7;
            this.textBoxIPs.Text = "192.168.0.165";
            this.textBoxIPs.TextChanged += new System.EventHandler(this.textBoxIPs_TextChanged);
            // 
            // checkBoxFlashOnChange
            // 
            this.checkBoxFlashOnChange.AutoSize = true;
            this.checkBoxFlashOnChange.Location = new System.Drawing.Point(363, 315);
            this.checkBoxFlashOnChange.Name = "checkBoxFlashOnChange";
            this.checkBoxFlashOnChange.Size = new System.Drawing.Size(158, 17);
            this.checkBoxFlashOnChange.TabIndex = 8;
            this.checkBoxFlashOnChange.Text = "Flash latest build on change";
            this.checkBoxFlashOnChange.UseVisualStyleBackColor = true;
            this.checkBoxFlashOnChange.CheckedChanged += new System.EventHandler(this.checkBoxFlashOnChange_CheckedChanged);
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(14, 171);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(32, 13);
            this.labelState.TabIndex = 9;
            this.labelState.Text = "State";
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(17, 187);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(504, 122);
            this.listView1.TabIndex = 10;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(439, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Open Log";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Github API key:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(17, 34);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(504, 20);
            this.textBoxPassword.TabIndex = 13;
            this.textBoxPassword.Text = "??????????????";
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(135, 315);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(222, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Clear last build flags and start updating";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PROTATool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 479);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.checkBoxFlashOnChange);
            this.Controls.Add(this.textBoxIPs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPR);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PROTATool";
            this.Text = "PROTATool";
            this.Load += new System.EventHandler(this.PROTATool_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIPs;
        private System.Windows.Forms.CheckBox checkBoxFlashOnChange;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button button2;
    }
}

