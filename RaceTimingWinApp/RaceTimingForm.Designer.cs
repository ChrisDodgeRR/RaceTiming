﻿namespace RedRat.RaceTimingWinApp
{
    partial class RaceTimingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RaceTimingForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importRunnerDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backupDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportEntrantsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.raceDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.raceEntrybrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.raceEntrantsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFinishPositionsbrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listAllFinishersbrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prizeWinnersbrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teamResultsbrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.raceStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clockControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetRaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.spaceBarLabel = new System.Windows.Forms.Label();
            this.resultListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.exportEmailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.timingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(863, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.importRunnerDataToolStripMenuItem,
            this.backupDBToolStripMenuItem,
            this.cSVExportToolStripMenuItem,
            this.toolStripSeparator2,
            this.optionsToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.newToolStripMenuItem.Text = "New Race";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItemClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.openToolStripMenuItem.Text = "Open Race File";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(175, 6);
            // 
            // importRunnerDataToolStripMenuItem
            // 
            this.importRunnerDataToolStripMenuItem.Name = "importRunnerDataToolStripMenuItem";
            this.importRunnerDataToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.importRunnerDataToolStripMenuItem.Text = "Import Runner Data";
            this.importRunnerDataToolStripMenuItem.Click += new System.EventHandler(this.ImportRunnerDataToolStripMenuItemClick);
            // 
            // backupDBToolStripMenuItem
            // 
            this.backupDBToolStripMenuItem.Name = "backupDBToolStripMenuItem";
            this.backupDBToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.backupDBToolStripMenuItem.Text = "Backup DB";
            this.backupDBToolStripMenuItem.Click += new System.EventHandler(this.BackupDbToolStripMenuItemClick);
            // 
            // cSVExportToolStripMenuItem
            // 
            this.cSVExportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportEntrantsToolStripMenuItem,
            this.exportResultsToolStripMenuItem,
            this.exportEmailsToolStripMenuItem});
            this.cSVExportToolStripMenuItem.Name = "cSVExportToolStripMenuItem";
            this.cSVExportToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.cSVExportToolStripMenuItem.Text = "CSV Export";
            // 
            // exportEntrantsToolStripMenuItem
            // 
            this.exportEntrantsToolStripMenuItem.Name = "exportEntrantsToolStripMenuItem";
            this.exportEntrantsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exportEntrantsToolStripMenuItem.Text = "Export Entrants";
            this.exportEntrantsToolStripMenuItem.Click += new System.EventHandler(this.ExportEntrantsToolStripMenuItemClick);
            // 
            // exportResultsToolStripMenuItem
            // 
            this.exportResultsToolStripMenuItem.Name = "exportResultsToolStripMenuItem";
            this.exportResultsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exportResultsToolStripMenuItem.Text = "Export Results";
            this.exportResultsToolStripMenuItem.Click += new System.EventHandler(this.ExportResultsToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(175, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.raceDetailsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // raceDetailsToolStripMenuItem
            // 
            this.raceDetailsToolStripMenuItem.Name = "raceDetailsToolStripMenuItem";
            this.raceDetailsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.raceDetailsToolStripMenuItem.Text = "Race Details";
            this.raceDetailsToolStripMenuItem.Click += new System.EventHandler(this.RaceDetailsToolStripMenuItemClick);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homePageToolStripMenuItem,
            this.toolStripSeparator4,
            this.raceEntrybrowserToolStripMenuItem,
            this.raceEntrantsToolStripMenuItem,
            this.addFinishPositionsbrowserToolStripMenuItem,
            this.listAllFinishersbrowserToolStripMenuItem,
            this.prizeWinnersbrowserToolStripMenuItem,
            this.teamResultsbrowserToolStripMenuItem,
            this.toolStripSeparator5,
            this.raceStatsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // homePageToolStripMenuItem
            // 
            this.homePageToolStripMenuItem.Name = "homePageToolStripMenuItem";
            this.homePageToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.homePageToolStripMenuItem.Text = "Home Page (browser)";
            this.homePageToolStripMenuItem.Click += new System.EventHandler(this.HomePageToolStripMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(236, 6);
            // 
            // raceEntrybrowserToolStripMenuItem
            // 
            this.raceEntrybrowserToolStripMenuItem.Name = "raceEntrybrowserToolStripMenuItem";
            this.raceEntrybrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.raceEntrybrowserToolStripMenuItem.Text = "Race Entry (browser)";
            this.raceEntrybrowserToolStripMenuItem.Click += new System.EventHandler(this.RaceEntrybrowserToolStripMenuItemClick);
            // 
            // raceEntrantsToolStripMenuItem
            // 
            this.raceEntrantsToolStripMenuItem.Name = "raceEntrantsToolStripMenuItem";
            this.raceEntrantsToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.raceEntrantsToolStripMenuItem.Text = "List Race Entrants (browser)";
            this.raceEntrantsToolStripMenuItem.Click += new System.EventHandler(this.RaceEntrantsToolStripMenuItemClick);
            // 
            // addFinishPositionsbrowserToolStripMenuItem
            // 
            this.addFinishPositionsbrowserToolStripMenuItem.Name = "addFinishPositionsbrowserToolStripMenuItem";
            this.addFinishPositionsbrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.addFinishPositionsbrowserToolStripMenuItem.Text = "Finish Positions Entry (browser)";
            this.addFinishPositionsbrowserToolStripMenuItem.Click += new System.EventHandler(this.AddFinishPositionsbrowserToolStripMenuItemClick);
            // 
            // listAllFinishersbrowserToolStripMenuItem
            // 
            this.listAllFinishersbrowserToolStripMenuItem.Name = "listAllFinishersbrowserToolStripMenuItem";
            this.listAllFinishersbrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.listAllFinishersbrowserToolStripMenuItem.Text = "List All Finishers (browser)";
            this.listAllFinishersbrowserToolStripMenuItem.Click += new System.EventHandler(this.ListAllFinishersbrowserToolStripMenuItemClick);
            // 
            // prizeWinnersbrowserToolStripMenuItem
            // 
            this.prizeWinnersbrowserToolStripMenuItem.Name = "prizeWinnersbrowserToolStripMenuItem";
            this.prizeWinnersbrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.prizeWinnersbrowserToolStripMenuItem.Text = "Prize Winners (browser)";
            this.prizeWinnersbrowserToolStripMenuItem.Click += new System.EventHandler(this.PrizeWinnersbrowserToolStripMenuItemClick);
            // 
            // teamResultsbrowserToolStripMenuItem
            // 
            this.teamResultsbrowserToolStripMenuItem.Name = "teamResultsbrowserToolStripMenuItem";
            this.teamResultsbrowserToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.teamResultsbrowserToolStripMenuItem.Text = "Team Results (browser)";
            this.teamResultsbrowserToolStripMenuItem.Click += new System.EventHandler(this.TeamResultsbrowserToolStripMenuItemClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(236, 6);
            // 
            // raceStatsToolStripMenuItem
            // 
            this.raceStatsToolStripMenuItem.Name = "raceStatsToolStripMenuItem";
            this.raceStatsToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.raceStatsToolStripMenuItem.Text = "Race Stats";
            this.raceStatsToolStripMenuItem.Click += new System.EventHandler(this.RaceStatsToolStripMenuItemClick);
            // 
            // timingToolStripMenuItem
            // 
            this.timingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clockControlToolStripMenuItem,
            this.resetRaceToolStripMenuItem});
            this.timingToolStripMenuItem.Name = "timingToolStripMenuItem";
            this.timingToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.timingToolStripMenuItem.Text = "Timing";
            // 
            // clockControlToolStripMenuItem
            // 
            this.clockControlToolStripMenuItem.Name = "clockControlToolStripMenuItem";
            this.clockControlToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.clockControlToolStripMenuItem.Text = "Clock Control";
            this.clockControlToolStripMenuItem.Click += new System.EventHandler(this.ClockControlToolStripMenuItemClick);
            // 
            // resetRaceToolStripMenuItem
            // 
            this.resetRaceToolStripMenuItem.Name = "resetRaceToolStripMenuItem";
            this.resetRaceToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.resetRaceToolStripMenuItem.Text = "Reset Race";
            this.resetRaceToolStripMenuItem.Click += new System.EventHandler(this.ResetRaceToolStripMenuItemClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.spaceBarLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultListView);
            this.splitContainer1.Size = new System.Drawing.Size(863, 345);
            this.splitContainer1.SplitterDistance = 557;
            this.splitContainer1.TabIndex = 2;
            // 
            // spaceBarLabel
            // 
            this.spaceBarLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.spaceBarLabel.AutoSize = true;
            this.spaceBarLabel.BackColor = System.Drawing.SystemColors.Control;
            this.spaceBarLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceBarLabel.Location = new System.Drawing.Point(54, 272);
            this.spaceBarLabel.Name = "spaceBarLabel";
            this.spaceBarLabel.Size = new System.Drawing.Size(449, 39);
            this.spaceBarLabel.TabIndex = 0;
            this.spaceBarLabel.Text = "Hit space bar to record times";
            this.spaceBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // resultListView
            // 
            this.resultListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.resultListView.FullRowSelect = true;
            this.resultListView.GridLines = true;
            this.resultListView.Location = new System.Drawing.Point(3, 3);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(296, 339);
            this.resultListView.TabIndex = 0;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Position";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Time";
            this.columnHeader2.Width = 102;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Race Number";
            this.columnHeader3.Width = 97;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "";
            this.columnHeader4.Width = 24;
            // 
            // exportEmailsToolStripMenuItem
            // 
            this.exportEmailsToolStripMenuItem.Name = "exportEmailsToolStripMenuItem";
            this.exportEmailsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exportEmailsToolStripMenuItem.Text = "Export Emails";
            this.exportEmailsToolStripMenuItem.Click += new System.EventHandler(this.ExportEmailsToolStripMenuItemClick);
            // 
            // RaceTimingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 369);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "RaceTimingForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Race Timing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RaceTimingFormFormClosing);
            this.Shown += new System.EventHandler(this.RaceTimingFormShown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RaceTimingFormKeyPress);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importRunnerDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem raceDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem raceEntrantsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clockControlToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label spaceBarLabel;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem homePageToolStripMenuItem;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripMenuItem resetRaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFinishPositionsbrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem raceEntrybrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listAllFinishersbrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prizeWinnersbrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backupDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teamResultsbrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cSVExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportEntrantsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem raceStatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportEmailsToolStripMenuItem;
    }
}

