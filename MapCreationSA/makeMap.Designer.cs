namespace MapCreationSA
{
    partial class makeMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(makeMap));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cbLevel = new System.Windows.Forms.ComboBox();
            this.pbMapMaking = new System.Windows.Forms.ProgressBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbMapType = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dtpFirst = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpLast = new System.Windows.Forms.DateTimePicker();
            this.btMakeMap = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tbMapName = new System.Windows.Forms.TextBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pbDownloading = new System.Windows.Forms.ProgressBar();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(198, 362);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.pbMapMaking);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btMakeMap);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(190, 333);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Побудова карт";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cbLevel);
            this.groupBox7.Location = new System.Drawing.Point(9, 159);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(176, 51);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Рівень";
            // 
            // cbLevel
            // 
            this.cbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLevel.FormattingEnabled = true;
            this.cbLevel.Location = new System.Drawing.Point(6, 19);
            this.cbLevel.Name = "cbLevel";
            this.cbLevel.Size = new System.Drawing.Size(160, 21);
            this.cbLevel.TabIndex = 20;
            this.cbLevel.SelectedIndexChanged += new System.EventHandler(this.cbLevel_SelectedIndexChanged);
            // 
            // pbMapMaking
            // 
            this.pbMapMaking.Location = new System.Drawing.Point(9, 306);
            this.pbMapMaking.Name = "pbMapMaking";
            this.pbMapMaking.Size = new System.Drawing.Size(176, 23);
            this.pbMapMaking.Step = 1;
            this.pbMapMaking.TabIndex = 16;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbMapType);
            this.groupBox3.Location = new System.Drawing.Point(9, 105);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(176, 48);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Вид карти";
            // 
            // cbMapType
            // 
            this.cbMapType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.cbMapType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMapType.FormattingEnabled = true;
            this.cbMapType.Items.AddRange(new object[] {
            "Середній геопотенціал",
            "Строковий геопотенціал",
            "Середня температура",
            "Середній тиск",
            "Строковий тиск",
            "Осьові ізогіпси",
            "Геопотенціал АТ500 сер. доб.",
            "ОТ500/1000",
            "Добова температура",
            "Експериментальне середнє"});
            this.cbMapType.Location = new System.Drawing.Point(6, 19);
            this.cbMapType.Name = "cbMapType";
            this.cbMapType.Size = new System.Drawing.Size(160, 21);
            this.cbMapType.TabIndex = 20;
            this.cbMapType.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dtpFirst);
            this.groupBox6.Location = new System.Drawing.Point(9, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(176, 47);
            this.groupBox6.TabIndex = 13;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Від";
            // 
            // dtpFirst
            // 
            this.dtpFirst.Location = new System.Drawing.Point(6, 17);
            this.dtpFirst.Name = "dtpFirst";
            this.dtpFirst.Size = new System.Drawing.Size(160, 20);
            this.dtpFirst.TabIndex = 6;
            this.dtpFirst.Value = new System.DateTime(2015, 8, 15, 11, 2, 0, 0);
            this.dtpFirst.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpLast);
            this.groupBox1.Location = new System.Drawing.Point(9, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 50);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "До";
            // 
            // dtpLast
            // 
            this.dtpLast.Location = new System.Drawing.Point(6, 19);
            this.dtpLast.Name = "dtpLast";
            this.dtpLast.Size = new System.Drawing.Size(160, 20);
            this.dtpLast.TabIndex = 6;
            this.dtpLast.Value = new System.DateTime(2015, 8, 25, 11, 2, 0, 0);
            this.dtpLast.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // btMakeMap
            // 
            this.btMakeMap.Location = new System.Drawing.Point(9, 273);
            this.btMakeMap.Name = "btMakeMap";
            this.btMakeMap.Size = new System.Drawing.Size(176, 30);
            this.btMakeMap.TabIndex = 9;
            this.btMakeMap.Text = "Створити карту";
            this.btMakeMap.UseVisualStyleBackColor = true;
            this.btMakeMap.Click += new System.EventHandler(this.btMakeMap_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tbMapName);
            this.groupBox5.Controls.Add(this.comboBox4);
            this.groupBox5.Location = new System.Drawing.Point(9, 216);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(176, 51);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Назва вихідного файлу";
            // 
            // tbMapName
            // 
            this.tbMapName.Location = new System.Drawing.Point(6, 20);
            this.tbMapName.Multiline = true;
            this.tbMapName.Name = "tbMapName";
            this.tbMapName.Size = new System.Drawing.Size(164, 21);
            this.tbMapName.TabIndex = 3;
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "536",
            "544",
            "560",
            "572",
            "556",
            "548"});
            this.comboBox4.Location = new System.Drawing.Point(6, 20);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(164, 21);
            this.comboBox4.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.pbDownloading);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(190, 333);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Оновлення даних";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 203);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 23);
            this.button3.TabIndex = 26;
            this.button3.Text = "Форматувати файл";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(18, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "Ост. дата: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(11, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(176, 48);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Вид даних";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Геопотенціал АТ500 строковий",
            "Геопотенціал АТ500 добовий",
            "Температура АТ500 добова",
            "Приземний тиск строковий"});
            this.comboBox1.Location = new System.Drawing.Point(6, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(160, 21);
            this.comboBox1.TabIndex = 26;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbYear);
            this.groupBox4.Location = new System.Drawing.Point(11, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(176, 46);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Рік";
            // 
            // cbYear
            // 
            this.cbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(8, 16);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(160, 21);
            this.cbYear.TabIndex = 27;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(18, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Прогрес: ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 111);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 25);
            this.button1.TabIndex = 21;
            this.button1.Text = "Завантажити дані";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbDownloading
            // 
            this.pbDownloading.Location = new System.Drawing.Point(16, 142);
            this.pbDownloading.Name = "pbDownloading";
            this.pbDownloading.Size = new System.Drawing.Size(160, 23);
            this.pbDownloading.TabIndex = 22;
            // 
            // makeMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 371);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "makeMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Побудова карт";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbMapType;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.DateTimePicker dtpFirst;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpLast;
        private System.Windows.Forms.Button btMakeMap;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tbMapName;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar pbDownloading;
        private System.Windows.Forms.ProgressBar pbMapMaking;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ComboBox cbLevel;
    }
}