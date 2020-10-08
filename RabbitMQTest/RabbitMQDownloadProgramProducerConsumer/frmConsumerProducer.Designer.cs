namespace RabbitMQDownloadProgramProducerConsumer
{
	partial class frmConsumerProducer
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
			this.cmdRequestProgram = new System.Windows.Forms.Button();
			this.txtProgramName = new System.Windows.Forms.TextBox();
			this.lblProgramName = new System.Windows.Forms.Label();
			this.txtProgramPath = new System.Windows.Forms.TextBox();
			this.lblProgramPath = new System.Windows.Forms.Label();
			this.txtProgramBody = new System.Windows.Forms.TextBox();
			this.lblProgramBody = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cmdRequestProgram
			// 
			this.cmdRequestProgram.Location = new System.Drawing.Point(431, 8);
			this.cmdRequestProgram.Name = "cmdRequestProgram";
			this.cmdRequestProgram.Size = new System.Drawing.Size(145, 42);
			this.cmdRequestProgram.TabIndex = 0;
			this.cmdRequestProgram.Text = "Invio richiesta programma";
			this.cmdRequestProgram.UseVisualStyleBackColor = true;
			this.cmdRequestProgram.Click += new System.EventHandler(this.cmdRequestProgram_Click);
			// 
			// txtProgramName
			// 
			this.txtProgramName.Location = new System.Drawing.Point(12, 27);
			this.txtProgramName.Name = "txtProgramName";
			this.txtProgramName.Size = new System.Drawing.Size(106, 23);
			this.txtProgramName.TabIndex = 1;
			// 
			// lblProgramName
			// 
			this.lblProgramName.AutoSize = true;
			this.lblProgramName.Location = new System.Drawing.Point(12, 9);
			this.lblProgramName.Name = "lblProgramName";
			this.lblProgramName.Size = new System.Drawing.Size(106, 15);
			this.lblProgramName.TabIndex = 2;
			this.lblProgramName.Text = "Nome Programma";
			// 
			// txtProgramPath
			// 
			this.txtProgramPath.Location = new System.Drawing.Point(124, 27);
			this.txtProgramPath.Name = "txtProgramPath";
			this.txtProgramPath.Size = new System.Drawing.Size(301, 23);
			this.txtProgramPath.TabIndex = 1;
			this.txtProgramPath.Text = "E:\\OSL\\PROGRAMMI\\";
			// 
			// lblProgramPath
			// 
			this.lblProgramPath.AutoSize = true;
			this.lblProgramPath.Location = new System.Drawing.Point(124, 9);
			this.lblProgramPath.Name = "lblProgramPath";
			this.lblProgramPath.Size = new System.Drawing.Size(119, 15);
			this.lblProgramPath.TabIndex = 2;
			this.lblProgramPath.Text = "Percorso Programma";
			// 
			// txtProgramBody
			// 
			this.txtProgramBody.Location = new System.Drawing.Point(12, 71);
			this.txtProgramBody.Multiline = true;
			this.txtProgramBody.Name = "txtProgramBody";
			this.txtProgramBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtProgramBody.Size = new System.Drawing.Size(564, 351);
			this.txtProgramBody.TabIndex = 1;
			// 
			// lblProgramBody
			// 
			this.lblProgramBody.AutoSize = true;
			this.lblProgramBody.Location = new System.Drawing.Point(12, 53);
			this.lblProgramBody.Name = "lblProgramBody";
			this.lblProgramBody.Size = new System.Drawing.Size(100, 15);
			this.lblProgramBody.TabIndex = 2;
			this.lblProgramBody.Text = "Testo Programma";
			// 
			// frmConsumerProducer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(585, 434);
			this.Controls.Add(this.lblProgramBody);
			this.Controls.Add(this.txtProgramBody);
			this.Controls.Add(this.lblProgramPath);
			this.Controls.Add(this.txtProgramPath);
			this.Controls.Add(this.lblProgramName);
			this.Controls.Add(this.txtProgramName);
			this.Controls.Add(this.cmdRequestProgram);
			this.Name = "frmConsumerProducer";
			this.Text = "Consumer-Producer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConsumerProducer_FormClosing);
			this.Load += new System.EventHandler(this.cmdRequestProgram_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cmdRequestProgram;
		private System.Windows.Forms.TextBox txtProgramName;
		private System.Windows.Forms.Label lblProgramName;
		private System.Windows.Forms.TextBox txtProgramPath;
		private System.Windows.Forms.Label lblProgramPath;
		private System.Windows.Forms.TextBox txtProgramBody;
		private System.Windows.Forms.Label lblProgramBody;
	}
}

