namespace QuickOpenFile
{
    partial class OptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.uxOk = new System.Windows.Forms.Button();
            this.uxCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.uxCheckSpaceAsWildcard = new System.Windows.Forms.CheckBox();
            this.uxCheckSearchInTheMiddle = new System.Windows.Forms.CheckBox();
            this.uxCheckCamelCase = new System.Windows.Forms.CheckBox();
            this.uxCheckIgnoreExternalDependencies = new System.Windows.Forms.CheckBox();
            this.uxCheckIgnorePatterns = new System.Windows.Forms.CheckBox();
            this.uxTextIgnorePatterns = new System.Windows.Forms.TextBox();
            this.uxCheckLimitResults = new System.Windows.Forms.CheckBox();
            this.uxTextLimitResults = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.uxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.uxCheckOpenMulti = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxOk
            // 
            this.uxOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxOk.Location = new System.Drawing.Point(306, 265);
            this.uxOk.Name = "uxOk";
            this.uxOk.Size = new System.Drawing.Size(85, 25);
            this.uxOk.TabIndex = 0;
            this.uxOk.Text = "OK";
            this.uxOk.UseVisualStyleBackColor = true;
            this.uxOk.Click += new System.EventHandler(this.uxOk_Click);
            // 
            // uxCancel
            // 
            this.uxCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.uxCancel.Location = new System.Drawing.Point(397, 265);
            this.uxCancel.Name = "uxCancel";
            this.uxCancel.Size = new System.Drawing.Size(85, 25);
            this.uxCancel.TabIndex = 1;
            this.uxCancel.Text = "Cancel";
            this.uxCancel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.uxCheckSpaceAsWildcard);
            this.flowLayoutPanel1.Controls.Add(this.uxCheckSearchInTheMiddle);
            this.flowLayoutPanel1.Controls.Add(this.uxCheckCamelCase);
            this.flowLayoutPanel1.Controls.Add(this.uxCheckIgnoreExternalDependencies);
            this.flowLayoutPanel1.Controls.Add(this.uxCheckIgnorePatterns);
            this.flowLayoutPanel1.Controls.Add(this.uxTextIgnorePatterns);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(464, 133);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // uxCheckSpaceAsWildcard
            // 
            this.uxCheckSpaceAsWildcard.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.uxCheckSpaceAsWildcard, true);
            this.uxCheckSpaceAsWildcard.Location = new System.Drawing.Point(9, 9);
            this.uxCheckSpaceAsWildcard.Name = "uxCheckSpaceAsWildcard";
            this.uxCheckSpaceAsWildcard.Size = new System.Drawing.Size(149, 17);
            this.uxCheckSpaceAsWildcard.TabIndex = 0;
            this.uxCheckSpaceAsWildcard.Text = "Treat space as * wildcard.";
            this.uxToolTip.SetToolTip(this.uxCheckSpaceAsWildcard, "If unchecked, a space in the search expression will search for explicit space in " +
        "the file name.");
            this.uxCheckSpaceAsWildcard.UseVisualStyleBackColor = true;
            // 
            // uxCheckSearchInTheMiddle
            // 
            this.uxCheckSearchInTheMiddle.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.uxCheckSearchInTheMiddle, true);
            this.uxCheckSearchInTheMiddle.Location = new System.Drawing.Point(9, 32);
            this.uxCheckSearchInTheMiddle.Name = "uxCheckSearchInTheMiddle";
            this.uxCheckSearchInTheMiddle.Size = new System.Drawing.Size(269, 17);
            this.uxCheckSearchInTheMiddle.TabIndex = 1;
            this.uxCheckSearchInTheMiddle.Text = "Search for the expression anywhere in the filename.";
            this.uxToolTip.SetToolTip(this.uxCheckSearchInTheMiddle, "If unchecked, only file names that start with the search expression will be shown" +
        ".");
            this.uxCheckSearchInTheMiddle.UseVisualStyleBackColor = true;
            // 
            // uxCheckCamelCase
            // 
            this.uxCheckCamelCase.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.uxCheckCamelCase, true);
            this.uxCheckCamelCase.Location = new System.Drawing.Point(9, 55);
            this.uxCheckCamelCase.Name = "uxCheckCamelCase";
            this.uxCheckCamelCase.Size = new System.Drawing.Size(175, 17);
            this.uxCheckCamelCase.TabIndex = 2;
            this.uxCheckCamelCase.Text = "Use CamelCase pattern search.";
            this.uxToolTip.SetToolTip(this.uxCheckCamelCase, "When checked, expressions like \"MeBo\" or \"MB\" will match camel case file names su" +
        "ch as \"MessageBox.cs\".");
            this.uxCheckCamelCase.UseVisualStyleBackColor = true;
            // 
            // uxCheckIgnoreExternalDependencies
            // 
            this.uxCheckIgnoreExternalDependencies.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.uxCheckIgnoreExternalDependencies, true);
            this.uxCheckIgnoreExternalDependencies.Location = new System.Drawing.Point(9, 78);
            this.uxCheckIgnoreExternalDependencies.Name = "uxCheckIgnoreExternalDependencies";
            this.uxCheckIgnoreExternalDependencies.Size = new System.Drawing.Size(169, 17);
            this.uxCheckIgnoreExternalDependencies.TabIndex = 3;
            this.uxCheckIgnoreExternalDependencies.Text = "Ignore external dependencies.";
            this.uxToolTip.SetToolTip(this.uxCheckIgnoreExternalDependencies, "Ignore files listed by Intellisense in External Dependencies folders.");
            this.uxCheckIgnoreExternalDependencies.UseVisualStyleBackColor = true;
            // 
            // uxCheckIgnorePatterns
            // 
            this.uxCheckIgnorePatterns.AutoSize = true;
            this.uxCheckIgnorePatterns.Location = new System.Drawing.Point(9, 101);
            this.uxCheckIgnorePatterns.Name = "uxCheckIgnorePatterns";
            this.uxCheckIgnorePatterns.Size = new System.Drawing.Size(222, 17);
            this.uxCheckIgnorePatterns.TabIndex = 7;
            this.uxCheckIgnorePatterns.Text = "Ignore patterns (separate with semicolon):";
            this.uxToolTip.SetToolTip(this.uxCheckIgnorePatterns, "If a file name matches one of these patterns, it will not be included in the resu" +
        "lt list. All search options (eg. camel case) apply also to this negative match.");
            this.uxCheckIgnorePatterns.UseVisualStyleBackColor = true;
            this.uxCheckIgnorePatterns.CheckedChanged += new System.EventHandler(this.uxCheckIgnorePatterns_CheckedChanged);
            // 
            // uxTextIgnorePatterns
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.uxTextIgnorePatterns, true);
            this.uxTextIgnorePatterns.Location = new System.Drawing.Point(234, 99);
            this.uxTextIgnorePatterns.Margin = new System.Windows.Forms.Padding(0, 1, 3, 3);
            this.uxTextIgnorePatterns.Name = "uxTextIgnorePatterns";
            this.uxTextIgnorePatterns.Size = new System.Drawing.Size(206, 20);
            this.uxTextIgnorePatterns.TabIndex = 8;
            // 
            // uxCheckLimitResults
            // 
            this.uxCheckLimitResults.AutoSize = true;
            this.uxCheckLimitResults.Location = new System.Drawing.Point(9, 9);
            this.uxCheckLimitResults.Name = "uxCheckLimitResults";
            this.uxCheckLimitResults.Size = new System.Drawing.Size(141, 17);
            this.uxCheckLimitResults.TabIndex = 4;
            this.uxCheckLimitResults.Text = "Limit result display to first";
            this.uxToolTip.SetToolTip(this.uxCheckLimitResults, "Maximum number of search results to display. This increases performance when sear" +
        "ching in large solutions.");
            this.uxCheckLimitResults.UseVisualStyleBackColor = true;
            this.uxCheckLimitResults.CheckedChanged += new System.EventHandler(this.uxCheckLimitResults_CheckedChanged);
            // 
            // uxTextLimitResults
            // 
            this.uxTextLimitResults.Location = new System.Drawing.Point(153, 7);
            this.uxTextLimitResults.Margin = new System.Windows.Forms.Padding(0, 1, 3, 3);
            this.uxTextLimitResults.Name = "uxTextLimitResults";
            this.uxTextLimitResults.Size = new System.Drawing.Size(51, 20);
            this.uxTextLimitResults.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.flowLayoutPanel2.SetFlowBreak(this.label1, true);
            this.label1.Location = new System.Drawing.Point(213, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 4, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "results.";
            // 
            // uxCheckOpenMulti
            // 
            this.uxCheckOpenMulti.AutoSize = true;
            this.uxCheckOpenMulti.Location = new System.Drawing.Point(9, 33);
            this.uxCheckOpenMulti.Name = "uxCheckOpenMulti";
            this.uxCheckOpenMulti.Size = new System.Drawing.Size(114, 17);
            this.uxCheckOpenMulti.TabIndex = 7;
            this.uxCheckOpenMulti.Text = "Open multiple files.";
            this.uxToolTip.SetToolTip(this.uxCheckOpenMulti, "When checked, displays a checkbox next to each search result. Checked files are o" +
        "pened in single step.");
            this.uxCheckOpenMulti.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 152);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Options";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.flowLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(15, 170);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 81);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Performance / Misc.";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.uxCheckLimitResults);
            this.flowLayoutPanel2.Controls.Add(this.uxTextLimitResults);
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.uxCheckOpenMulti);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(6);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(458, 62);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.uxOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.uxCancel;
            this.ClientSize = new System.Drawing.Size(494, 302);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.uxCancel);
            this.Controls.Add(this.uxOk);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quick Open File Options";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.OptionsDialog_HelpButtonClicked);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uxOk;
        private System.Windows.Forms.Button uxCancel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox uxCheckSpaceAsWildcard;
        private System.Windows.Forms.CheckBox uxCheckSearchInTheMiddle;
        private System.Windows.Forms.CheckBox uxCheckCamelCase;
        private System.Windows.Forms.CheckBox uxCheckIgnoreExternalDependencies;
        private System.Windows.Forms.CheckBox uxCheckLimitResults;
        private System.Windows.Forms.TextBox uxTextLimitResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox uxCheckIgnorePatterns;
        private System.Windows.Forms.TextBox uxTextIgnorePatterns;
        private System.Windows.Forms.ToolTip uxToolTip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox uxCheckOpenMulti;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}