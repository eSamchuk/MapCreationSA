using System;
using System.Collections.Generic;
using System.ComponentModel;
using sysd = System.Data;
using System.IO;
using System.Net;
using System.Drawing;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using wf = System.Windows.Forms;
using Microsoft.Research.Science.Data;
using Surfer;
using Microsoft.Research.Science.Data.Imperative;

namespace MapCreationSA
{
    public partial class makeMap : wf.Form
    {
        private string _ncLocation;
        public string ncLocation
        {
            get { return _ncLocation; }
            set { _ncLocation = value; }
        }

        private string _mapLocation;
        public string mapLocation
        {
            get { return _mapLocation; }
            set { _mapLocation = value; }
        }

        private string _centerLocation;
        public string centerLocation
        {
            get { return _centerLocation; }
            set { _centerLocation = value; }
        }

        private level _level;
        public level Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private string _output;
        public string output
        {
            get { return _output; }
            set { _output = value; }
        }

        List<mapPoint> mapPoints { get; set; }

        private string LastFileCreated { get; set; }



        public makeMap()
        {
            InitializeComponent();
            this.StartPosition = wf.FormStartPosition.CenterScreen;
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            mapPoints = new List<mapPoint>();
            getXYValues();

            for (int i = DateTime.Now.Year; i >= 2000; cbYear.Items.Add((i--).ToString()));

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini";
            StringBuilder returnString = new StringBuilder(255);

            iniReader.GetPrivateProfileString("map_location", "path", "", returnString, returnString.Capacity, path);
            mapLocation = returnString.ToString();

            iniReader.GetPrivateProfileString("nc_location", "path", "", returnString, returnString.Capacity, path);
            ncLocation = returnString.ToString();

            iniReader.GetPrivateProfileString("mdf_location", "centr", "", returnString, returnString.Capacity, path);
            centerLocation = returnString.ToString();

            iniReader.GetPrivateProfileString("map_location", "path2", "", returnString, returnString.Capacity, path);
            output = returnString.ToString();

            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            cbYear.SelectedIndexChanged -= cbYear_SelectedIndexChanged;

            comboBox1.SelectedIndex = 0;
            cbYear.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            cbYear.SelectedIndexChanged += cbYear_SelectedIndexChanged;

            cbLevel.Items.Add(new level("Земля", 0));
            cbLevel.Items.Add(new level("850", 2));
            cbLevel.Items.Add(new level("700", 3));
            cbLevel.Items.Add(new level("500", 5));
        }

        private void btMakeMap_Click(object sender, EventArgs e)
        {
            Thread th1 = new Thread(makeMeanMap);
            switch(cbMapType.SelectedIndex)
            {
                case 1:
                    th1 = new Thread(makeHgtStrok);
                    break;
                case 0:
                case 3:
                case 2:
                    th1 = new Thread(makeMeanMap);
                    break;
                case 4:
                    th1 = new Thread(makeSlpStrok);
                    break;
                case 5:
                    th1 = new Thread(makeIsoMap);
                    break;
                case 6:
                    th1 = new Thread(makeHgtDaily);
                    break;
                case 7:
                    th1 = new Thread(makeOT);
                    break;
                case 8:
                    th1 = new Thread(makeTempDaily);
                    break;
                //case 9:
                //    th1 = new Thread(makeExperimentalMean);
                //    break;
            }
            th1.Start();
        }

        public void getXYValues()
        {
            string pathh = AppDomain.CurrentDomain.BaseDirectory + @"block_meta.mdf";
            SqlConnection sc = new SqlConnection($@"Data Source=.\SQLEXPRESS;AttachDbFilename={pathh};Integrated Security=True;Connect Timeout=30;User Instance=True");
            sc.Open();

            SqlCommand cm = new SqlCommand();
            cm.CommandText = "SELECT row, col, X, Y FROM Nodes";
            cm.Connection = sc;
            cm.CommandType = sysd.CommandType.Text;

            SqlDataReader sqlReader = cm.ExecuteReader();
            
            while (sqlReader.Read()) {
                mapPoints.Add(new mapPoint(sqlReader.GetInt32(2), sqlReader.GetInt32(3), sqlReader.GetInt32(0), sqlReader.GetInt32(1)));
            }

            sqlReader.Close();
            sc.Close();
        }

        public void makeSlpStrok()
        {
            string folder = null, map_name = null;

            createMapName(out folder, out map_name);

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "slp";
            int l = 0, i = 0, j = 0, k = 1, ll = dtpFirst.Value.DayOfYear, fday = dtpFirst.Value.DayOfYear, lday = dtpLast.Value.DayOfYear;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            Single[, ,] res_slp = null;
           
            pbMapMaking.Value = 0;
            this.Cursor = wf.Cursors.WaitCursor;
            dd = null;
            mon = null;

            int ii = 0;
            Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
            Surfer.IShapes Shapes = Doc.Shapes;

            pbDownloading.Value = 0;
            int year = dtpFirst.Value.Year;
            l = 0;
            pokaznyk = "slp";
            path = ncLocation + @"\" + pokaznyk + @"\" + pokaznyk + "." + year + ".nc";
         
            DataSet ds = DataSet.Open(path);

            string[] st = new string[5];
            st[1] = "00";
            st[2] = "06";
            st[3] = "12";
            st[4] = "18";

            res_slp = null;
            res_slp = ds.GetData<Single[, ,]>(4);

            int start = dtpFirst.Value.DayOfYear * 4 - 4;
            int fin = dtpLast.Value.DayOfYear * 4;

            l = dtpFirst.Value.DayOfYear;

            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
            }

            pbMapMaking.Maximum = fin - start + 1;

            k = 1;
            for (i = start; i < fin; i++) // цикл по днях  
            {

                if (i + 1 < 10) path = mapLocation + year + @"\" + pokaznyk + @"\" + year + "_0" + l.ToString() + "_" + st[k];
                else path = mapLocation + year + @"\" + pokaznyk + @"\" + year + "_" + l.ToString() + "_" + st[k];

                if (File.Exists(path + "_slp.txt") == true) File.Delete(path + "_slp.txt");

                for (j = 0; j < mapPoints.Count ; j++)
                {
                    using (StreamWriter sw = new StreamWriter(path + "_slp.txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(mapPoints[j].X.ToString() + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1).ToString());
                    }
                }

                if (File.Exists(pathh + "_slp.grd") == true) File.Delete(pathh + "_slp.grd");
                AppSurfer.GridData2(path + "_slp.txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                ii = l;
                for (int q = 1; q <= 12; q++)
                {
                    if (ii <= DateTime.DaysInMonth(year, q))
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - DateTime.DaysInMonth(year, q);
                }

                MapFrame = Shapes.AddContourMap(path + "_slp.grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                ContourMap.Levels.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "lvl\\slp_2.lvl");

                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top; //7.932062; //8.257119;
                MapFrame.Left = Shapes.Item(1).Left; //0.774817; //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; // 4.875000;//
                MapFrame.Width = Shapes.Item(1).Width;  //6.945067;  ////

                ContourMap.LabelFont.Size = 8;
                ContourMap.LabelLabelDist = 3.0;
                ContourMap.FillContours = true;

                if (Convert.ToInt32(dd) < 10) dd = "0" + dd;
                if (Convert.ToInt32(mon) < 10) mon = "0" + mon;


                MapFrame.Axes.Item(1).Title = "Тиск на рівні моря в строк " + st[k] + " UTC " + dd + "." + mon + "." + year;
                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);

                k++;
                if (k > 4)
                {
                    l = l + 1;
                    k = 1;
                }

                Doc.Export2(path + "_slp.jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");

                File.Delete(path + "_slp.jpg.gsr2");
                File.Delete(path + "_slp.grd");
                File.Delete(path + "_slp.txt");
                MapFrame.Delete();
                pbMapMaking.Value = pbMapMaking.Value + 1;
            
            }
            this.Cursor = wf.Cursors.Default;

            exitSurferApp();

        }
        //public void makeExperimentalMean()
        //{
        //    this.Cursor = wf.Cursors.WaitCursor;

        //    string folder = null, map_name = null;

        //    create_name(out folder, out map_name);

        //    string yr = dtpFirst.Value.Year.ToString();
        //    string path = null;
        //    int i = 0, j = 0;
        //    int ll = dtpFirst.Value.DayOfYear;
        //    int fday = dtpFirst.Value.DayOfYear;
        //    int lday = dtpLast.Value.DayOfYear;
        //    string mont = dtpFirst.Value.Month.ToString();
        //    int diff = lday - fday + 1;

        //    Single[,,,] res = null;
        //    double[] mean = new double[mapPoints.Count];

        //    double [,] temp_field = new double[29, 144];

        //    StringBuilder returnString = new StringBuilder(255);
           
        //    int year = dtpFirst.Value.Year;

        //    path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";

        //    DataSet ds = DataSet.Open(path);

        //    res = ds.GetData<Single[,,,]>(5);

        //    double[,] max = new double[29, 144], min = new double[29, 144]; //middle = 0;
        //    double tmp_max = 0, tmp_min = 6000, tmp_middle = 0;


        //    for (int fi = 0; fi < 29; fi++)
        //        for (int lamb = 0; lamb < 144; lamb++)
        //            min[fi, lamb] = 6000;
             

        //    if (dtpFirst.Value.Year == dtpLast.Value.Year)
        //    {
        //        for (i = fday - 1; i <= lday - 1; i++)
        //        {
        //            for (int fi = 0; fi < 29; fi++)
        //            {
        //                for (int lamb = 0; lamb < 144; lamb++)
        //                {
        //                    if (res[i, 5, fi, lamb] > max[fi, lamb])
        //                    {
        //                        max[fi, lamb] = res[i, 5, fi, lamb];
        //                        if (max[fi, lamb] > tmp_max) tmp_max = max[fi, lamb];
        //                    }
        //                    if (res[i, 5, fi, lamb] < min[fi, lamb])
        //                    {
        //                        min[fi, lamb] = res[i, 5, fi, lamb];
        //                        if (min[fi, lamb] < tmp_min) tmp_min = min[fi, lamb];
        //                    }
        //                }
        //            }
        //        }    
        //    }

        //    tmp_middle = (tmp_max + tmp_min) / 2;

        //    if (tmp_middle < 5360) tmp_middle = 5360;

        //    //for (int fi = 0; fi < 29; fi++)
        //    //{
        //    //    for (int lamb = 0; lamb < 144; lamb++)
        //    //    {
        //    //        if (res[i, 5, fi, lamb] >= tmp_middle) temp_field[fi, lamb] = Math.Round(max[fi, lamb] * 0.1, 1);
        //    //        else temp_field[fi, lamb] = Math.Round(min[fi, lamb] * 0.1, 1);
        //    //    }
        //    //}

        //    path = @"E:\Science\Centr_tracking\Blocks\" + year + "\\" + tbMapName.Text + "\\" + tbMapName.Text + "_experimantal";
        //    if (File.Exists(path + "_max.txt")) File.Delete(path + "_max.txt");
        //    if (File.Exists(path + "_min.txt")) File.Delete(path + "_min.txt");
        //    if (File.Exists(path + "_avg.txt")) File.Delete(path + "_avg.txt");

        //    for (j = 0; j < mapPoints.Count; j++)
        //    {
        //        using (StreamWriter sw = new StreamWriter(path + "_max.txt", true, Encoding.UTF8))
        //        {
        //            double d1 = mapPoints[j].Row;
        //            double d2 = mapPoints[j].Col;
        //            sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + (max[mapPoints[j].Row, mapPoints[j].Col] * .1).ToString());
        //        }

        //    }

        //    for (j = 0; j < mapPoints.Count; j++)
        //    {
        //        using (StreamWriter sw = new StreamWriter(path + "_min.txt", true, Encoding.UTF8))
        //        {
        //            double d1 = mapPoints[j].Row;
        //            double d2 = mapPoints[j].Col;
        //            sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + (min[mapPoints[j].Row, mapPoints[j].Col] * .1).ToString());
        //        }

        //    }

        //    for (j = 0; j < mapPoints.Count; j++)
        //    {
        //        using (StreamWriter sw = new StreamWriter(path + "_avg.txt", true, Encoding.UTF8))
        //        {
        //            double d1 = mapPoints[j].Row;
        //            double d2 = mapPoints[j].Col;
        //            sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + ((max[mapPoints[j].Row, mapPoints[j].Col] - min[mapPoints[j].Row, mapPoints[j].Col]) * .1).ToString());
        //        }

        //    }


        //    //else
        //    //{
        //    //    int endDOY = new DateTime(dateTimePicker1.Value.DayOfYear, 12, 31).DayOfYear;
        //    //    if (comboBox3.SelectedIndex == 3) endDOY *= 4;
        //    //    for (i = fday - 1; i < endDOY; i++)
        //    //    {
        //    //        for (j = 0; j < mapPoints.Count; j++)
        //    //        {
        //    //            mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
        //    //        }
        //    //    }
        //    //    diff = endDOY - fday + 1;
        //    //    path = ncLocation + @"hgtDaily\hgt." + (dateTimePicker1.Value.Year + 1).ToString() + ".nc";
        //    //    fday = dateTimePicker2.Value.DayOfYear - 1;
        //    //    ds = DataSet.Open(path);
        //    //    res = ds.GetData<Single[,,,]>(5);
        //    //    for (i = 0; i <= fday; i++)
        //    //    {
        //    //        for (j = 0; j < mapPoints.Count; j++)
        //    //        {
        //    //            mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
        //    //        }
        //    //    }
        //    //    diff += fday + 1;
        //    //}

        //    //Surfer.Application AppSurfer = new Surfer.Application();
        //    //AppSurfer.Visible = true;
        //    //Surfer.IPlotDocument Doc;
        //    //Surfer.IMapFrame MapFrame;

        //    //Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");

        //    //double mmmax = mean.Max();

        //    //if (File.Exists(path + ".grd") == true) File.Delete(path + ".grd");
        //    //AppSurfer.GridData2(path + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);
        //    //Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
        //    //Surfer.IShapes Shapes = Doc.Shapes;

        //    //MapFrame = Shapes.AddContourMap(path + ".grd");
        //    //Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1); ;
        //    //Surfer.ILevels levels = ContourMap.Levels;

        //    //MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(1).ShowLabels = false;

        //    //MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(2).ShowLabels = false;

        //    //MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(3).ShowLabels = false;

        //    //MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
        //    //MapFrame.Axes.Item(4).ShowLabels = false;

        //    //MapFrame.Top = Shapes.Item(1).Top;
        //    //MapFrame.Left = Shapes.Item(1).Left;

        //    //MapFrame.Height = Shapes.Item(1).Height;
        //    //MapFrame.Width = Shapes.Item(1).Width;

        //    //if (comboBox3.SelectedIndex == 0) MapFrame.Axes.Item(1).Title = "Середня карта АТ500 за період " + folder;
        //    //if (comboBox3.SelectedIndex == 2) MapFrame.Axes.Item(1).Title = "Середня температура на рівні АТ500 за період " + folder;
        //    //if (comboBox3.SelectedIndex == 3) MapFrame.Axes.Item(1).Title = "Середній приземний тиск за період " + folder;

        //    //MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
        //    //Doc.Export2(path + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, PixelsPerInch=300");
        //    //File.Delete(path + ".jpg.gsr2");
        //    ////File.Delete(path + ".txt");
        //    ////File.Delete(path + ".grd");

        //    //MapFrame.Delete();

        //    this.Cursor = wf.Cursors.Default;

        //    //exitSurferApp();
        //}
        public void makeMeanMap()
        {
            string folder = null, map_name = null, yr = dtpFirst.Value.Year.ToString(), path = null, pathh = null, mont = dtpFirst.Value.Month.ToString();

            createMapName(out folder, out map_name);

            int i = 0, j = 0, ll = dtpFirst.Value.DayOfYear, fday = dtpFirst.Value.DayOfYear, lday = dtpLast.Value.DayOfYear, diff = lday - fday + 1;
            
            Surfer.Application AppSurfer = new Surfer.Application();
            
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            Single[, ,] res_slp = null;
            Single[, , ,] res = null;
            double[] mean = new double[mapPoints.Count];

            pbMapMaking.Value = 0;
            this.Cursor = wf.Cursors.WaitCursor;

            Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");

            if (cbMapType.SelectedIndex == 0)
            {
                path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
                if (Directory.Exists(mapLocation + @"\" + yr + @"\hgt\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\hgt\");
            }

            if (cbMapType.SelectedIndex == 2)
            {
                path = ncLocation + @"airDaily\air." + yr + ".nc";
                if (Directory.Exists(mapLocation + @"\" + yr + @"\air\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\air\");
            }
            if (cbMapType.SelectedIndex == 3)
            {
                if (Directory.Exists(mapLocation + @"\" + yr + @"\slp\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\slp\");
                path = ncLocation + @"slp\slp." + yr + ".nc";
                fday = fday * 4 - 4 + 1;
                lday = lday * 4;
                diff = lday - fday + 1;
            }

            DataSet ds = DataSet.Open(path);

            if (cbMapType.SelectedIndex == 4 | cbMapType.SelectedIndex == 3) res_slp = ds.GetData<Single[, ,]>(4);
            else res = ds.GetData<Single[, , ,]>(5);
    
            pbDownloading.Value = 0;

            if (dtpFirst.Value.Year == dtpLast.Value.Year)
            {
                for (i = fday - 1; i <= lday - 1; i++)
                {
                    for (j = 0; j < mapPoints.Count ; j++)
                    {
                        switch (cbMapType.SelectedIndex)
                        {
                            case 0:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                                break;
                            case 2:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                                break;
                            case 3:
                                mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                                break;
                        }
                    }
                }
            }
            else
            {
                int endDOY = new DateTime(dtpFirst.Value.DayOfYear, 12, 31).DayOfYear;
                if (cbMapType.SelectedIndex == 3) endDOY *= 4;

                for (i = fday - 1; i < endDOY; i++)
                {
                    for (j = 0; j < mapPoints.Count; j++)
                    {
                        switch (cbMapType.SelectedIndex)
                        {
                            case 0:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                                break;
                            case 2:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                                break;
                            case 3:
                                mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                                break;
                        }
                    }
                }

                diff = endDOY - fday + 1;


                switch (cbMapType.SelectedIndex)
                {
                    case 0:
                        path = ncLocation + @"hgtDaily\hgt." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                        fday = dtpLast.Value.DayOfYear - 1;
                        break;
                    case 2:
                        path = ncLocation + @"airDaily\air." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                        fday = dtpLast.Value.DayOfYear - 1;
                        break;
                    case 3:
                        path = ncLocation + @"slp\slp." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                        fday = dtpLast.Value.DayOfYear * 4 - 1;
                        break;
                }

                ds = DataSet.Open(path);

                if (cbMapType.SelectedIndex == 4 | cbMapType.SelectedIndex == 3) res_slp = ds.GetData<Single[, ,]>(4);
                else res = ds.GetData<Single[, , ,]>(5);

                for (i = 0; i <= fday; i++)
                {
                    for (j = 0; j < mapPoints.Count; j++)
                    {
                        switch (cbMapType.SelectedIndex)
                        {
                            case 0:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                                break;
                            case 2:
                                mean[j] += Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                                break;
                            case 3:
                                mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                                break;
                        }
                    }
                }

                diff += fday + 1;
            }

            if (mont.Length == 1) path = output + @"\" + yr + "_0" + mont + @"\" + folder;
            else path = output + @"\" + yr + "_" + mont + @"\" + folder;
            //int VFZ = Convert.ToInt32(comboBox4.Text);

            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);

            pathh = path + @"\" + map_name;
            if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");
            for (j = 0; j < mapPoints.Count ; j++)
            {
                mean[j] = mean[j] / diff;
                using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + mean[j]);
                }
            }

            this.Cursor = wf.Cursors.WaitCursor;

            double mmmin = mean.Min();

            int min = Convert.ToInt32(mmmin / 5) * 5;

            double mmmax = mean.Max();
            int max = Convert.ToInt32(mmmax / 5) * 5;
            if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
            AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);
            Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
            Surfer.IShapes Shapes = Doc.Shapes;

            path = output + tbMapName.Text;
            MapFrame = Shapes.AddContourMap(pathh + ".grd");
            Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1); ;
            Surfer.ILevels levels = ContourMap.Levels;


            //contourMap ajustment
            {
                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;
                MapFrame.Left = Shapes.Item(1).Left;

                MapFrame.Height = Shapes.Item(1).Height;
                MapFrame.Width = Shapes.Item(1).Width;

            }

            switch (cbMapType.SelectedIndex)
            {
                case 0:

                    switch (Level.arrIndex)
                    {
                        case 5:
                            ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot500.lvl");
                            break;
                        case 3:
                            ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot700.lvl");
                            break;
                        case 2:
                            ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot850.lvl");
                            break;
                    }

                    ContourMap.FillContours = true;
                    MapFrame.Axes.Item(1).Title = $"Середня карта АТ{Level.height} за період " + folder;
                    break;
                case 2:
                    {
                        if (min > mmmin) min -= 5;

                        ContourMap.Levels.AutoGenerate(min, max, 5.0);
                        ContourMap.FillForegroundColorMap.LoadFile(wf.Application.StartupPath + @"\lvl\rainbow.clr");

                        for (j = 1; j <= ContourMap.Levels.Count; j++)
                        {
                            ContourMap.Levels.Item(j).ShowLabel = true;
                        }

                        ContourMap.ApplyFillToLevels(1, (Math.Abs(max - min) / 5) - 2, 0);
                        ContourMap.FillContours = true;
                        MapFrame.Axes.Item(1).Title = $"Середня температура на рівні АТ{Level.height} за період " + folder;
                    }
                    break;
                case 3:
                    ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\slp_2.lvl");
                    ContourMap.FillContours = true;
                    MapFrame.Axes.Item(1).Title = "Середній приземний тиск за період " + folder;
                    break;
            }

        

            //for (j = 1; j <= ContourMap.Levels.Count; j++)
            //{
            //    if (ContourMap.Levels.Item(j).Value == VFZ)
            //    {
            //        ContourMap.Levels.Item(j).Line.Width = 0.05;
            //        break;
            //    }
            //}

            MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
            Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
            File.Delete(pathh + ".jpg.gsr2");
            File.Delete(pathh + ".txt");
            File.Delete(pathh + ".grd");
            Process.Start($"{pathh}.jpg");

            MapFrame.Delete();

            this.Cursor = wf.Cursors.Default;
            exitSurferApp();
        }
        public void makeHgtStrok()
        {
            string folder = null, map_name = null;

            createMapName(out folder, out map_name);

            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "hgt", level = Level.height;
            int i = 0, j = 0, ll = dtpFirst.Value.DayOfYear, fday = dtpFirst.Value.DayOfYear, lday = dtpLast.Value.DayOfYear;
            int year = dtpFirst.Value.Year;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            Single[, , ,] res = null;
         
            if (Directory.Exists(mapLocation + @"\" + year) == false) Directory.CreateDirectory(mapLocation + @"\" + year);
           
            path = ncLocation + @"hgt\hgt." + year + ".nc";

            DataSet ds = DataSet.Open(path);
            res = ds.GetData<Single[, , ,]>(5);
 
            if (Directory.Exists(mapLocation + @"\" + year + @"\hgt\") == false) Directory.CreateDirectory(mapLocation + @"\" + year + @"\hgt\");
            fday = fday * 4 - 4;
            lday = lday * 4 - 1;
            pbMapMaking.Maximum = lday - fday + 1;
    
            string[] st = new string[5];
            st[1] = "00";
            st[2] = "06";
            st[3] = "12";
            st[4] = "18";

            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            int kl = 1;

            for (i = fday; i <= lday; i++)
            {

                int ii = ll;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= DateTime.DaysInMonth(year, q))
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - DateTime.DaysInMonth(year, q);
                }

                if (ll <= 9) pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_0" + ll.ToString() + "_" + st[kl] + "_h" + level;
                else pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_" + ll.ToString() + "_" + st[kl] + "_h" + level;

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");

                for (j = 0; j < mapPoints.Count; j++)
                {
                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(res[i, Level.arrIndex, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1).ToString());
                    }
                }

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                Surfer.ILevels levels = ContourMap.Levels;


                switch (Level.arrIndex)
                {
                    case 2:
                        ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot850.lvl");
                        break;
                    case 3:
                        ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot700.lvl");
                        break;
                    case 5:
                        ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot500.lvl");
                        break;
                }

                ContourMap.FillContours = true;

                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;          //8.257119;
                MapFrame.Left = Shapes.Item(1).Left;       //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; //8.254075;
                MapFrame.Width = Shapes.Item(1).Width;  //11.698747;

                if (dd.Length == 1) dd = "0" + dd;
                if (mon.Length == 1) mon = "0" + mon;

                //for (j = 1; j <= ContourMap.Levels.Count; j++)
                //{
                //    if (ContourMap.Levels.Item(j).Value == VFZ)
                //    {

                //        ContourMap.Levels.Item(j).Line.Width = 0.05;
                //    }
                //}
            
                MapFrame.Axes.Item(1).Title = $"Геопотенціал АТ{Level.height} в строк {st[kl]}UTC {dd}.{mon}.{year}";
             
                kl++;
                if (kl >= 5)
                {
                    ll = ll + 1;
                    kl = 1;
                }

                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
                Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                File.Delete(pathh + ".jpg.gsr2");
                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                MapFrame.Delete();
                this.Cursor = wf.Cursors.Default;
            }
            exitSurferApp();
        }
        public void makeHgtDaily()
        {
            int[] row = new int[2449];
            int[] col = new int[2449];
            int[] x = new int[2449];
            int[] y = new int[2449];

            string folder = null, map_name = null;

            createMapName(out folder, out map_name);

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "hgtDaily", level = "500";
            int i = 0, j = 0, lvl = 5, diff = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;

            int[] monthdays = new int[13];
            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            Single[, , ,] res = null;

            if (Directory.Exists(mapLocation + @"\" + yr) == false) Directory.CreateDirectory(mapLocation + @"\" + yr);

            path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";

            DataSet ds = DataSet.Open(path);
            res = ds.GetData<Single[, , ,]>(5);

            if (Directory.Exists(mapLocation + @"\" + yr + @"\hgtDaily\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\hgtDaily\");
            diff = lday - fday;

            string[] st = new string[5];
            st[1] = "00";
            st[2] = "06";
            st[3] = "12";
            st[4] = "18";

            string year = dtpFirst.Value.Year.ToString();

            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }


            if (Convert.ToInt32(year) % 4 == 0) monthdays[2] = 29;
            else monthdays[2] = 28;

            for (i = fday - 1; i <= lday - 1; i++)
            {

                int ii = ll;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= monthdays[q])
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - monthdays[q];
                }

                if (ll <= 9) pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_0" + ll.ToString() + "_h" + level;
                else pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_" + ll.ToString() + "_h" + level;

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");

                for (j = 0; j < mapPoints.Count ; j++)
                {
                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(res[i, lvl, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1).ToString());
                    }

                }

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                Surfer.ILevels levels = ContourMap.Levels;

                ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot500.lvl");
                ContourMap.FillContours = true;

                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;          //8.257119;
                MapFrame.Left = Shapes.Item(1).Left;       //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; //8.254075;
                MapFrame.Width = Shapes.Item(1).Width;  //11.698747;

                if (Convert.ToInt32(dd) < 10) dd = "0" + dd;
                if (Convert.ToInt32(mon) < 10) mon = "0" + mon;

                //for (j = 1; j <= ContourMap.Levels.Count; j++)
                //{
                //    if (ContourMap.Levels.Item(j).Value == VFZ)
                //    {

                //        ContourMap.Levels.Item(j).Line.Width = 0.05;
                //    }
                //}

                MapFrame.Axes.Item(1).Title = "Середній геопотенціал АТ500 за " + dd + "." + mon + "." + year;
                ll++;
          
                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
                Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                File.Delete(pathh + ".jpg.gsr2");
                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                MapFrame.Delete();
                this.Cursor = wf.Cursors.Default;

            }
            exitSurferApp();
        }
        public void makeTempDaily()
        {
            string folder = null, map_name = null;
            createMapName(out folder, out map_name);

            int[] monthdays = new int[13];
            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "airDaily", level = "500";
            int i = 0, j = 0, lvl = 5, diff = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            double[] mean = new double[mapPoints.Count];
            Single[, , ,] res = null;

            if (Directory.Exists(mapLocation + @"\" + yr) == false) Directory.CreateDirectory(mapLocation + @"\" + yr);

            path = ncLocation + @"airDaily\air." + yr + ".nc";

            DataSet ds = DataSet.Open(path);
            res = ds.GetData<Single[, , ,]>(5);

            if (Directory.Exists(mapLocation + @"\" + yr + @"\airDaily\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\airDaily\");
            diff = lday - fday;

            string[] st = new string[5];
            st[1] = "00";
            st[2] = "06";
            st[3] = "12";
            st[4] = "18";

            string year = dtpFirst.Value.Year.ToString();

            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }


            if (Convert.ToInt32(year) % 4 == 0) monthdays[2] = 29;
            else monthdays[2] = 28;

            for (i = fday - 1; i <= lday - 1; i++)
            {

                int ii = ll;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= monthdays[q])
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - monthdays[q];
                }

                if (ll <= 9) pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_0" + ll.ToString() + "_t" + level;
                else pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_" + ll.ToString() + "_t" + level;

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");

                for (j = 0; j < mapPoints.Count ; j++)
                {
                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        mean[j] = Math.Round(res[i, lvl, mapPoints[j].Row, mapPoints[j].Col] - 273);
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(res[i, lvl, mapPoints[j].Row, mapPoints[j].Col] - 273, 1).ToString());
                    }
                }

                double mmmin = mean.Min();
                int min = Convert.ToInt32(mmmin / 4) * 4;

                double mmmax = mean.Max();
                int max = Convert.ToInt32(mmmax / 4) * 4;

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                Surfer.ILevels levels = ContourMap.Levels;

                if (min > mmmin) min -= 4;

                ContourMap.Levels.AutoGenerate(min, max, 4.0);
                ContourMap.FillForegroundColorMap.LoadFile(wf.Application.StartupPath + @"\lvl\rainbow.clr");

                for (j = 1; j <= ContourMap.Levels.Count; j+= 1)
                {
                    ContourMap.Levels.Item(j).ShowLabel = true;
                }

                ContourMap.ApplyFillToLevels(1, (Math.Abs(max - min) / 4) - 2, 0);
                ContourMap.FillContours = true;

                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;         
                MapFrame.Left = Shapes.Item(1).Left; 

                MapFrame.Height = Shapes.Item(1).Height;
                MapFrame.Width = Shapes.Item(1).Width; 

                if (Convert.ToInt32(dd) < 10) dd = "0" + dd;
                if (Convert.ToInt32(mon) < 10) mon = "0" + mon;


                MapFrame.Axes.Item(1).Title = "Середня температура АТ" + level + " за " + dd + "." + mon + "." + year;
                ll++;

                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
                Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                File.Delete(pathh + ".jpg.gsr2");
                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                MapFrame.Delete();
                this.Cursor = wf.Cursors.Default;

            }
            exitSurferApp();
        }
        public void makeOT()
        {
            string folder = null, map_name = null;
            createMapName(out folder, out map_name);

            int[] monthdays = new int[13];
            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "OT";
            int i = 0, j = 0, diff = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            double[] hgtt = new double[2449];
            int hgtn = 0;
            Single[,,,] res = null;


            if (Directory.Exists(mapLocation + @"\" + yr) == false) Directory.CreateDirectory(mapLocation + @"\" + yr);

            path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";

            DataSet ds = DataSet.Open(path);
            res = ds.GetData<Single[,,,]>(5);

            if (Directory.Exists(mapLocation + @"\" + yr + @"\OT\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\OT\");
            diff = lday - fday;

            string[] st = new string[5];
            st[1] = "00";
            st[2] = "06";
            st[3] = "12";
            st[4] = "18";

            string year = dtpFirst.Value.Year.ToString();

            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
            }

            if (Convert.ToInt32(year) % 4 == 0) monthdays[2] = 29;
            else monthdays[2] = 28;

            for (i = fday; i <= lday; i++)
            {

                int ii = ll;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= monthdays[q])
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - monthdays[q];
                }

                if (ll <= 9) pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + @"\" + year + "_0" + ll.ToString() + "_OT";
                else pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + @"\" + year + "_" + ll.ToString() + "_OT";

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");
                hgtn = 0;
                for (j = 0; j < mapPoints.Count ; j++)
                {
                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        double ddd = res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1 - res[i, 0, mapPoints[j].Row, mapPoints[j].Col] * 0.1;
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(ddd, 1).ToString());
                        hgtt[hgtn] = ddd;
                        hgtn++;
                    }

                }

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                Surfer.ILevels levels = ContourMap.Levels;


                double mmmin = hgtt.Min();

                int min = Convert.ToInt32(mmmin / 4) * 4;

                double mmmax = hgtt.Max();
                int max = Convert.ToInt32(mmmax / 4) * 4;


                ContourMap.Levels.AutoGenerate(min, max, 4.0);
                ContourMap.FillForegroundColorMap.LoadFile(wf.Application.StartupPath + @"\rainbow.clr");

                for (j = 1; j <= ContourMap.Levels.Count; j+=2)
                {
                    ContourMap.Levels.Item(j).ShowLabel = true;
                }

                ContourMap.ApplyFillToLevels(1, ContourMap.Levels.Count, 0);
                ContourMap.FillContours = true;

                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;          //8.257119;
                MapFrame.Left = Shapes.Item(1).Left;       //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; //8.254075;
                MapFrame.Width = Shapes.Item(1).Width;  //11.698747;

                if (Convert.ToInt32(dd) < 10) dd = "0" + dd;
                if (Convert.ToInt32(mon) < 10) mon = "0" + mon;

                MapFrame.Axes.Item(1).Title = "Топографія OT500/1000 за " + dd + "." + mon + "." + year;
                ll++;

                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
                Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                File.Delete(pathh + ".jpg.gsr2");
                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                MapFrame.Delete();
                this.Cursor = wf.Cursors.Default;

            }
            exitSurferApp();
        }
        public void makeIsoMap()
        {
            string folder = null, map_name = null;
            createMapName(out folder, out map_name);

            int[] monthdays = new int[13];
            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            Font font = new Font("Arial Black", 12.0F);
            Single[, , ,] res = null;
            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null;
            int l = 0, i = 0, j = 0, diff = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            string mont = dtpFirst.Value.Month.ToString();
            Brush brush = new SolidBrush(Color.Red);
            Pen rect_pen = new Pen(brush);

            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;

            Color[] clr2 = new Color[9];
            clr2[1] = Color.Black;
            clr2[2] = Color.Red;
            clr2[3] = Color.Blue;
            clr2[4] = Color.Green;
            clr2[5] = Color.Brown;
            clr2[6] = Color.FromArgb(153, 0, 153);
            clr2[7] = Color.FromArgb(255, 153, 51);
            clr2[8] = Color.FromArgb(204, 51, 255);
            int iso = Convert.ToInt32(comboBox4.Text);

            Surfer.srfColor[] cl = new srfColor[9];
            cl[1] = srfColor.srfColorBlack;
            cl[2] = srfColor.srfColorRed;
            cl[3] = srfColor.srfColorBlue;
            cl[4] = srfColor.srfColorGreen;
            cl[5] = srfColor.srfColorBrown;
            cl[6] = srfColor.srfColorLightViolet;
            cl[7] = srfColor.srfColorLightOrange;
            cl[8] = srfColor.srfColorNeonPurple;

            path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
            if (Directory.Exists(mapLocation + @"\" + yr + @"\hgt\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\hgt\");
            diff = lday - fday;

            DataSet ds = DataSet.Open(path);
            res = ds.GetData<Single[, , ,]>(5);

            l = 1;
            bool expand = false;
            if (dtpLast.Value.Year > dtpFirst.Value.Year)
            {
                lday = new DateTime(dtpFirst.Value.Year, 12, 31).DayOfYear;
                expand = true;
            }

            for (i = fday - 1; i <= lday - 1; i++)
            {

                if (i == lday - 1 && expand == true)
                {
                    path = ncLocation + @"hgtDaily\hgt." + dtpLast.Value.Year + ".nc";
                    ds = DataSet.Open(path);
                    res = ds.GetData<Single[, , ,]>(5);
                    i = 0;
                    lday = dtpLast.Value.DayOfYear;
                    expand = false;
                }

                int ii = i + 1;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= monthdays[q])
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - monthdays[q];
                }

                if (mont.Length == 1) pathh = output + @"\" + yr + "_0" + mont + @"\" + folder;
                else pathh = output + @"\" + yr + "_" + mont + @"\" + folder;

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");

                for (j = 0; j < mapPoints.Count ; j++)
                {

                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1).ToString());
                    }

                }

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");

                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1);
                Surfer.ILevels levels = ContourMap.Levels;

                ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot500.lvl");
                MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(1).ShowLabels = false;

                MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(2).ShowLabels = false;

                MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(3).ShowLabels = false;

                MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
                MapFrame.Axes.Item(4).ShowLabels = false;

                MapFrame.Top = Shapes.Item(1).Top;          //8.257119;
                MapFrame.Left = Shapes.Item(1).Left;       //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; //8.254075;
                MapFrame.Width = Shapes.Item(1).Width;  //11.698747;

                if (Convert.ToInt32(dd) < 10) dd = "0" + dd;
                if (Convert.ToInt32(mon) < 10) mon = "0" + mon;

                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);

                for (j = 1; j <= ContourMap.Levels.Count; j++)
                {
                    if (ContourMap.Levels.Item(j).Value != iso)
                    {
                        ContourMap.Levels.Item(j).ShowLabel = false;

                        ContourMap.Levels.Item(j).Line.Style = "Invisible";
                    }
                    else
                    {
                        ContourMap.Levels.Item(j).ShowLabel = false;
                        //ContourMap.Levels.Item(j).Line.Width = 0.01;

                        ContourMap.Levels.Item(j).Line.ForeColor = cl[l];
                        l++;
                        if (l > 8) l = 1;
                    }
                }

                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                if (i == lday - 1)
                {
                    MapFrame.Axes.Item(1).Title = "Осьові ізогіпси АТ500 за період " + map_name;

                    if (mont.Length == 1) path = output + yr + "_0" + mont;
                    else path = output + yr + "_" + mont;

                    Doc.Export2(path + @"\" + map_name + @"\" + map_name + "_iso_" + iso.ToString() + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                    File.Delete(path + @"\" + map_name + @"\" + map_name + "_iso_" + iso.ToString() + ".jpg.gsr2");
                    
                }
            }

            string path2 = path + @"\" + map_name + @"\" + map_name + "_iso_" + iso.ToString() + ".jpg";
            Bitmap myBitmap = new Bitmap(path2);
            Graphics graphics = Graphics.FromImage(myBitmap);
            expand = false;
            int l3 = dtpLast.Value.DayOfYear - dtpFirst.Value.DayOfYear + 1;

            if (dtpFirst.Value.DayOfYear > dtpLast.Value.DayOfYear)
            {
                lday = new DateTime(dtpFirst.Value.Year, 12, 31).DayOfYear;
                l3 = lday - fday + dtpLast.Value.DayOfYear + 1;
                expand = true;
            }

            yr = dtpFirst.Value.Year.ToString();
           
            l = 1;

            path2 = path + @"\" + map_name + @"\" + map_name + "_iso_" + iso.ToString() + ".png";
            myBitmap.Save(path2, ImageFormat.Png);
            myBitmap.Dispose();
            exitSurferApp();
            this.Cursor = wf.Cursors.Default;
        }
        public void makeTermAdvMap()
        {

            string folder = null, map_name = null;
            createMapName(out folder, out map_name);


            double[] duga = new double[29];

            int[] monthdays = new int[13];
            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null, mon = null, dd = null, pokaznyk = "tempAdv", level = "500";
            int l = 0, i = 0, j = 0, k = 0, lvl = 5, diff = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            Surfer.Application AppSurfer = new Surfer.Application();
            AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;

            Single[, , ,] resHgt = null;
            Single[, , ,] resV = null;
            Single[, , ,] resW = null;

            if (Directory.Exists(mapLocation + @"\" + yr) == false) Directory.CreateDirectory(mapLocation + @"\" + yr);

            path = ncLocation + @"airDaily\air." + yr + ".nc";
            DataSet ds = DataSet.Open(path);
            resHgt = ds.GetData<Single[, , ,]>(5);

            if (Directory.Exists(mapLocation + @"\" + yr + @"\tAdv\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\tAdv\");
            diff = lday - fday;

            Single[, ,] resHgt2 = new Single[diff + 1, 30, 144];
            Single[, ,] resV2 = new Single[diff + 1, 30, 144];
            Single[, ,] resW2 = new Single[diff + 1, 30, 144];
            Single[, ,] tempAdv = new Single[diff + 1, 29, 144];
            l = 0;
            for (i = fday; i <= lday; i++)
            {
                for (k = 0; k <= 29; k++)
                {
                    for (j = 0; j <= 143; j++)
                    {
                        resHgt2[l, k, j] = (float)Math.Round((resHgt[i, lvl, k, j] - 273.1), 1);
                    }
                }
                l++;
            }

            resHgt = null;
            GC.Collect();
           

            path = ncLocation + @"uwind\uwnd." + yr + ".nc";
            ds = DataSet.Open(path);
            resV = ds.GetData<Single[, , ,]>(5);

            l = 0;
            for (i = fday; i <= lday; i++)
            {
                for (k = 0; k <= 29; k++)
                {

                    for (j = 0; j <= 143; j++)
                    {
                        resV2[l, k, j] = (float)(Math.Round((resV[i, lvl, k, j]), 1));
                    }

                }
                l++;
            }

            resV = null;
            GC.Collect();
           

            path = ncLocation + @"vwind\vwnd." + yr + ".nc";
            ds = DataSet.Open(path);
            resW = ds.GetData<Single[, , ,]>(5);

            l = 0;
            for (i = fday; i <= lday; i++)
            {
                for (k = 0; k <= 29; k++)
                {

                    for (j = 0; j <= 143; j++)
                    {
                        resW2[l, k, j] = (float)(Math.Round((resW[i, lvl, k, j]), 1));
                    }

                }
                l++;
            }

            resW = null;
            GC.Collect();
            int ffin = l;

            double dtdx = 0.0, dtdy = 0.0;

            //розрахунок адвекції

            for (i = fday - 1; i <= lday - 1; i++)
            {
               
                for (j = 0; j <= 143; j++)
                {
                    for (k = 0; k <= 28; k++)
                    {
                        if (k > 0 & k <= 28)
                        {
                            dtdy = ((resHgt2[i, k + 1, j] - resHgt2[i, k - 1, j]) * 100) / (5 * 111.1);
                        }
                        else if (k == 0)
                        {
                            dtdy = ((resHgt2[i, k + 1, j] - resHgt2[i, k, j]) * 100) / (2.5 * 111.1);
                        }

                        if (k > 0)
                        {
                            if (j > 0 & j <= 142)
                            {
                                dtdx = ((resHgt2[i, k, j + 1] - resHgt2[i, k, j - 1]) * 100) / (5 * duga[k]);
                            }
                            else if (j == 0)
                            {
                                dtdx = ((resHgt2[i, k, 1] - resHgt2[i, k, 143]) * 100) / (5 * duga[k]);
                            }
                            else if (j == 143)
                            {
                                dtdx = ((resHgt2[i, k, 0] - resHgt2[i, k, 142]) * 100) / (5 * duga[k]);
                            }
                        }


                        tempAdv[i, k, j] = (float)(-(resV2[i, k, j] * dtdx + resW2[i, k, j] * dtdy)) / 24;

                    }
                }

            }

           

            string year = dtpFirst.Value.Year.ToString();
            if (Convert.ToInt32(year) % 4 == 0) monthdays[2] = 29;
            else monthdays[2] = 28;


            if (Directory.Exists(mapLocation + @"\" + year) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\");
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }

            if (Directory.Exists(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level) == false)
            {
                Directory.CreateDirectory(mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level);
            }


            for (i = 0; i < ffin; i++)
            {

                int ii = ll;
                for (int q = 1; q <= 12; q++)
                {

                    if (ii <= monthdays[q])
                    {
                        mon = q.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii = ii - monthdays[q];
                }

                if (ll <= 9) pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_0" + ll.ToString() + "_tempAdv" + level;
                else pathh = mapLocation + @"\" + year + @"\" + pokaznyk + @"\" + level + @"\" + year + "_" + ll.ToString() + "_"+ "_h" + level;

                if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");

                for (j = 0; j < mapPoints.Count ; j++)
                {
                    using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                    {
                        sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + Math.Round(tempAdv[i, mapPoints[j].Row, mapPoints[j].Col], 1).ToString());
                    }

                }

                this.Cursor = wf.Cursors.WaitCursor;

                if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
                AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);

                Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\MPK-7_full3.srf");
                Surfer.IShapes Shapes = Doc.Shapes;

                path = output + tbMapName.Text;
                MapFrame = Shapes.AddContourMap(pathh + ".grd");

                MapFrame.Axes.Item(1).Title = "Адвекція АТ500 за  " + dd + "." + mon + "." + year;
                Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1); ;
                Surfer.ILevels levels = ContourMap.Levels;
                ContourMap.FillForegroundColorMap.LoadFile(wf.Application.StartupPath + @"\rainbow.clr");

                for (j = 1; j <= ContourMap.Levels.Count; j++)
                {
                    ContourMap.Levels.Item(j).ShowLabel = true;
                }

                ContourMap.ApplyFillToLevels(1, ContourMap.Levels.Count, 0);
                ContourMap.FillContours = true;

                MapFrame.Top = Shapes.Item(1).Top;          //8.257119;
                MapFrame.Left = Shapes.Item(1).Left;       //- 18.061119;

                MapFrame.Height = Shapes.Item(1).Height; //8.254075;
                MapFrame.Width = Shapes.Item(1).Width;  //11.698747;

                MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
                Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
                File.Delete(pathh + ".jpg.gsr2");
                File.Delete(pathh + ".txt");
                File.Delete(pathh + ".grd");

                MapFrame.Delete();

                //AppSurfer.Quit();

                this.Cursor = wf.Cursors.Default;


                ll++;
            }



        }
        public void makeAnomalyMap()
        {
            string folder = null, map_name = null;

            createMapName(out folder, out map_name);

            string yr = dtpFirst.Value.Year.ToString();
            string path = null, pathh = null;
            int i = 0, j = 0;
            int ll = dtpFirst.Value.DayOfYear;
            int fday = dtpFirst.Value.DayOfYear;
            int lday = dtpLast.Value.DayOfYear;
            string mont = dtpFirst.Value.Month.ToString();
            int diff = lday - fday + 1;
            Surfer.Application AppSurfer = new Surfer.Application();

            //AppSurfer.Visible = true;
            Surfer.IPlotDocument Doc;
            Surfer.IMapFrame MapFrame;
            Single[,,] res_slp = null;
            Single[,,,] res = null;
            double[] mean = new double[mapPoints.Count];

            int[] monthdays = new int[13] {0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            pbMapMaking.Value = 0;
            this.Cursor = wf.Cursors.WaitCursor;

            Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");

            if (cbMapType.SelectedIndex == 0)
            {
                path = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
                if (Directory.Exists(mapLocation + @"\" + yr + @"\hgt\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\hgt\");
            }

            if (cbMapType.SelectedIndex == 2)
            {
                path = ncLocation + @"airDaily\air." + yr + ".nc";
                if (Directory.Exists(mapLocation + @"\" + yr + @"\air\") == false) Directory.CreateDirectory(mapLocation + @"\" + yr + @"\air\");
            }

            DataSet ds = DataSet.Open(path);

            if (cbMapType.SelectedIndex == 4 | cbMapType.SelectedIndex == 3) res_slp = ds.GetData<Single[,,]>(4);
            else res = ds.GetData<Single[,,,]>(5);

            pbDownloading.Value = 0;
            string year = dtpFirst.Value.Year.ToString();
            if (Convert.ToInt32(year) % 4 == 0) monthdays[2] = 29;
            else monthdays[2] = 28;

            if (dtpFirst.Value.Year == dtpLast.Value.Year)
            {
                for (i = fday - 1; i <= lday - 1; i++)
                {
                    for (j = 0; j < mapPoints.Count ; j++)
                    {
                        if (cbMapType.SelectedIndex == 0) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                        if (cbMapType.SelectedIndex == 2) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                        if (cbMapType.SelectedIndex == 3) mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                    }
                }
            }
            else
            {
                int endDOY = new DateTime(dtpFirst.Value.DayOfYear, 12, 31).DayOfYear;
                if (cbMapType.SelectedIndex == 3) endDOY *= 4;

                for (i = fday - 1; i < endDOY; i++)
                {
                    for (j = 0; j < mapPoints.Count; j++)
                    {
                        if (cbMapType.SelectedIndex == 0) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                        if (cbMapType.SelectedIndex == 2) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                        if (cbMapType.SelectedIndex == 3) mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                    }
                }

                diff = endDOY - fday + 1;

                if (cbMapType.SelectedIndex == 0)
                {
                    path = ncLocation + @"hgtDaily\hgt." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                    fday = dtpLast.Value.DayOfYear - 1;
                }
                if (cbMapType.SelectedIndex == 2)
                {
                    path = ncLocation + @"airDaily\air." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                    fday = dtpLast.Value.DayOfYear - 1;
                }
                if (cbMapType.SelectedIndex == 3)
                {
                    path = ncLocation + @"slp\slp." + (dtpFirst.Value.Year + 1).ToString() + ".nc";
                    fday = dtpLast.Value.DayOfYear * 4 - 1;
                }


                ds = DataSet.Open(path);

                if (cbMapType.SelectedIndex == 4 | cbMapType.SelectedIndex == 3) res_slp = ds.GetData<Single[,,]>(4);
                else res = ds.GetData<Single[,,,]>(5);

                for (i = 0; i <= fday; i++)
                {
                    for (j = 0; j < mapPoints.Count; j++)
                    {
                        if (cbMapType.SelectedIndex == 0) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] * 0.1, 1);
                        if (cbMapType.SelectedIndex == 2) mean[j] += Math.Round(res[i, 5, mapPoints[j].Row, mapPoints[j].Col] - 273.0, 1);
                        if (cbMapType.SelectedIndex == 3) mean[j] += Math.Round(res_slp[i, mapPoints[j].Row, mapPoints[j].Col] * 0.01, 1);
                    }
                }


                diff += fday + 1;

            }

            if (mont.Length == 1) path = output + @"\" + yr + "_0" + mont + @"\" + folder;
            else path = output + @"\" + yr + "_" + mont + @"\" + folder;
            int VFZ = Convert.ToInt32(comboBox4.Text);

            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);

            pathh = path + @"\" + map_name;
            if (File.Exists(pathh + ".txt") == true) File.Delete(pathh + ".txt");
            for (j = 0; j < mapPoints.Count ; j++)
            {
                mean[j] = mean[j] / diff;
                using (StreamWriter sw = new StreamWriter(pathh + ".txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(mapPoints[j].X + " " + (893 - mapPoints[j].Y).ToString() + " " + mean[j]);
                }
            }

            this.Cursor = wf.Cursors.WaitCursor;

            double mmmin = mean.Min();

            int min = Convert.ToInt32(mmmin / 5) * 5;

            double mmmax = mean.Max();
            int max = Convert.ToInt32(mmmax / 5) * 5;
            if (File.Exists(pathh + ".grd") == true) File.Delete(pathh + ".grd");
            AppSurfer.GridData2(pathh + ".txt", 1, 2, 3, DupMethod: SrfDupMethod.srfDupNone, xMin: 0, xMax: 1043, yMin: 0, yMax: 893, Algorithm: SrfGridAlgorithm.srfKriging, ShowReport: false);
            Doc = AppSurfer.Documents.Open(wf.Application.StartupPath + @"\lvl\MPK-7_full2.srf");
            Surfer.IShapes Shapes = Doc.Shapes;

            path = output + tbMapName.Text;
            MapFrame = Shapes.AddContourMap(pathh + ".grd");
            Surfer.IContourMap ContourMap = (Surfer.IContourMap)MapFrame.Overlays.Item(1); ;
            Surfer.ILevels levels = ContourMap.Levels;

            if (cbMapType.SelectedIndex == 0)
            {
                ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\heopot500.lvl");
                ContourMap.FillContours = true;
            }
            if (cbMapType.SelectedIndex == 2)
            {
                if (min > mmmin) min -= 5;

                ContourMap.Levels.AutoGenerate(min, max, 5.0);
                ContourMap.FillForegroundColorMap.LoadFile(wf.Application.StartupPath + @"\lvl\rainbow.clr");

                for (j = 1; j <= ContourMap.Levels.Count; j++)
                {
                    ContourMap.Levels.Item(j).ShowLabel = true;
                }

                ContourMap.ApplyFillToLevels(1, (Math.Abs(max - min) / 5) - 2, 0);
                ContourMap.FillContours = true;
            }

            if (cbMapType.SelectedIndex == 3)
            {
                ContourMap.Levels.LoadFile(wf.Application.StartupPath + @"\lvl\slp_2.lvl");
                ContourMap.FillContours = true;
            }

            MapFrame.Axes.Item(1).MajorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(1).MinorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(1).ShowLabels = false;

            MapFrame.Axes.Item(2).MajorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(2).MinorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(2).ShowLabels = false;

            MapFrame.Axes.Item(3).MajorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(3).MinorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(3).ShowLabels = false;

            MapFrame.Axes.Item(4).MajorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(4).MinorTickType = SrfTickType.srfTickNone;
            MapFrame.Axes.Item(4).ShowLabels = false;

            MapFrame.Top = Shapes.Item(1).Top;
            MapFrame.Left = Shapes.Item(1).Left;

            MapFrame.Height = Shapes.Item(1).Height;
            MapFrame.Width = Shapes.Item(1).Width;

            for (j = 1; j <= ContourMap.Levels.Count; j++)
            {
                if (ContourMap.Levels.Item(j).Value == VFZ)
                {
                    //ContourMap.Levels.Item(j).Line.Width = 0.05;
                }
            }

            if (cbMapType.SelectedIndex == 0) MapFrame.Axes.Item(1).Title = "Середня карта АТ500 за період " + folder;
            if (cbMapType.SelectedIndex == 2) MapFrame.Axes.Item(1).Title = "Середня температура на рівні АТ500 за період " + folder;
            if (cbMapType.SelectedIndex == 3) MapFrame.Axes.Item(1).Title = "Середній приземний тиск за період " + folder;

            MapFrame.SetZOrder(SrfZOrder.srfZOToBack);
            Doc.Export2(pathh + ".jpg", Options: "Defaults=1,Width=1043,KeepAspect=1,ColorDepth=24,Automatic=0,Quality=100, HDPI=300, VDPI=300");
            File.Delete(pathh + ".jpg.gsr2");
            File.Delete(pathh + ".txt");
            File.Delete(pathh + ".grd");

            //File.Copy(pathh + ".jpg", @"E:\Science\Scandinavia\" + (Convert.ToInt32(year) - 1990).ToString() + "_" + map_name + ".jpg", true);

            MapFrame.Delete();

            this.Cursor = wf.Cursors.Default;

            exitSurferApp();
        }
        public void exitSurferApp()
        {
            Process[] procList = Process.GetProcesses();
            for (int l = 0; l <= procList.Length - 1; l++)
            {

                if (procList[l].ToString() == "System.Diagnostics.Process (Surfer)")
                {
                    procList[l].Kill();
                }
            }

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
           
        }

        public void createMapName(out string folder, out string map_name)
        {
            string add = null;
            folder = null;
            map_name = null;
            if (cbMapType.SelectedIndex == 0) add = $"_АТ{Level.height}_h";
            if (cbMapType.SelectedIndex == 1) add = $"_АТ{Level.height}";
            if (cbMapType.SelectedIndex == 2) add = $"_АТ{Level.height}_t";
            if (cbMapType.SelectedIndex == 3) add = "_slp";

            if (dtpFirst.Value > dtpLast.Value)
            {
                //wf.MessageBox.Show("Межі періоду встановлені неправильно!");
                tbMapName.Text = "";
                goto fin;
            }

            if (dtpFirst.Value.Day == dtpLast.Value.Day & dtpFirst.Value.Month == dtpLast.Value.Month & dtpFirst.Value.Year == dtpLast.Value.Year)
            {
               // textBox2.Text = "";
               // wf.MessageBox.Show("Межі періоду встановлені неправильно!");
               //  goto fin;
            }



            if (dtpFirst.Value.Year == dtpLast.Value.Year)
            {

                if (dtpFirst.Value.Month == dtpLast.Value.Month)
                {
                    if (dtpFirst.Value.Day == dtpLast.Value.Day)
                    {
                        tbMapName.Text = dtpFirst.Value.ToString().Substring(0, 10) + add;
                        folder = dtpFirst.Value.ToString().Substring(0, 10);
                        map_name = tbMapName.Text;

                    }
                    else
                    {
                        tbMapName.Text = dtpFirst.Value.ToString().Substring(0, 2) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2) + add;
                        folder = dtpFirst.Value.ToString().Substring(0, 2) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2);
                        map_name = tbMapName.Text;
                    }
                }
                else
                {
                    tbMapName.Text = dtpFirst.Value.ToString().Substring(0, 5) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2) + add;
                    folder = dtpFirst.Value.ToString().Substring(0, 5) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2);
                    map_name = tbMapName.Text;
                }

            }
            else
            {
                tbMapName.Text = dtpFirst.Value.ToString().Substring(0, 6) + dtpFirst.Value.Year.ToString().Substring(2, 2) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2) + add;
                folder = dtpFirst.Value.ToString().Substring(0, 6) + dtpFirst.Value.Year.ToString().Substring(2, 2) + "-" + dtpLast.Value.ToString().Substring(0, 6) + dtpLast.Value.Year.ToString().Substring(2, 2);
                map_name = tbMapName.Text;
            }


        fin: ;

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMapType.SelectedIndex == 5) comboBox4.BringToFront();
            else comboBox4.SendToBack();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(DownloadFile);
            th.Start();
        }

        public void DownloadFile()
        {
            Uri URL = null;
            string yr = cbYear.Text.ToString();
            WebClient webClient;
            Stopwatch sw = new Stopwatch();
            string location = null;
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);


                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        URL = new Uri("ftp://ftp.cdc.noaa.gov/Datasets/ncep.reanalysis/pressure/hgt." + yr + ".nc");
                        location = ncLocation + @"hgt\hgt." + yr + ".nc";
                        break;
                    case 1:
                        URL = new Uri("ftp://ftp.cdc.noaa.gov/Datasets/ncep.reanalysis.dailyavgs/pressure/hgt." + yr + ".nc");
                        location = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
                        break;
                    case 2:
                        URL = new Uri("ftp://ftp.cdc.noaa.gov/Datasets/ncep.reanalysis.dailyavgs/pressure/air." + yr + ".nc");
                        location = ncLocation + @"airDaily\air." + yr + ".nc";
                        break;
                    case 3:
                        URL = new Uri("ftp://ftp.cdc.noaa.gov/Datasets/ncep.reanalysis/surface/slp." + yr + ".nc");
                        location = ncLocation + @"slp\slp." + yr + ".nc";
                        break;
                }

                try
                {
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    wf.MessageBox.Show(ex.Message);
                }
            }

        }

        public void checkLastDate()
        {
            label2.Text = "";
            int ii = 0, j = 0, yr = Convert.ToInt32(cbYear.Text);
            string dd = null, mon = null, location = null;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    location = ncLocation + @"hgt\hgt." + yr + ".nc";
                    break;
                case 1:
                    location = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
                    break;
                case 2:
                    location = ncLocation + @"airDaily\air." + yr + ".nc";
                    break;
                case 3:
                    location = ncLocation + @"slp\slp." + yr + ".nc";
                    break;
            }



            DataSet ds = DataSet.Open(location);
            Double[] time = null;

            if (comboBox1.SelectedIndex == 2) time = ds.GetData<Double[]>(3);
            else  time = ds.GetData<Double[]>(4);
            int Tbound = time.Length;
            ii = Tbound;

            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 3) { ii /= 4; }

                for (j = 1; j <= 12; j++)
                {

                    if (ii <= DateTime.DaysInMonth(yr, j))
                    {
                        mon = j.ToString();
                        dd = ii.ToString();
                        break;
                    }
                    else ii -= DateTime.DaysInMonth(yr, j);

                }
           
            if (dd.Length == 1) dd = "0" + dd;
            if (mon.Length < 10) mon = "0" + mon;

            label2.Text = $"Ост. дата: {dd}.{mon}.{yr.ToString()}";
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownloading.Value = e.ProgressPercentage;
            label1.Text = $"Прогрес: {e.ProgressPercentage}%";
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                wf.MessageBox.Show("Завантаження перервано!");
            }
            else
            {
                wf.MessageBox.Show("Завантаження успішно завершено!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int[] monthdays = new int[13];
            int j = 0, lday = 0, fday = 0, k = 0, dov = 0, sh = 0, lvl = 0, dd = 0, mon = 0;

            monthdays[1] = 31;
            monthdays[2] = 28;
            monthdays[3] = 31;
            monthdays[4] = 30;
            monthdays[5] = 31;
            monthdays[6] = 30;
            monthdays[7] = 31;
            monthdays[8] = 31;
            monthdays[9] = 30;
            monthdays[10] = 31;
            monthdays[11] = 30;
            monthdays[12] = 31;

            int yr = Convert.ToInt32(cbYear.Text);
            if (yr % 4 == 0) monthdays[2] = 29;


            int[] level = new int[17];

            level[0] = 1000;
            level[1] = 925;
            level[2] = 850;
            level[3] = 700;
            level[4] = 600;
            level[5] = 500;
            level[6] = 400;
            level[7] = 300;
            level[8] = 250;
            level[9] = 200;
            level[10] = 150;
            level[11] = 100;
            level[12] = 70;
            level[13] = 50;
            level[14] = 30;
            level[15] = 20;
            level[16] = 10;

            string location = null;
            Single[] lon = new Single[144];

            for (int df = 0; df < 144; df++)
            {
                lon[df] = (float)(df * 2.5);
            }

            Single[] lat = new Single[30];

            for (int df = 0; df < 30; df++)
            {
                lat[df] = (float)(df * -2.5) + 90;
            }

            string[] dims = new string[10];
            dims[0] = "time";
            dims[1] = "level";
            dims[2] = "lon";
            dims[3] = "lat";
            dims[4] = "nbnds";


            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    location = ncLocation + @"hgt\hgt." + yr + ".nc";
                    break;
                case 1:
                    location = ncLocation + @"hgtDaily\hgt." + yr + ".nc";
                    break;
                case 2:
                    location = ncLocation + @"airDaily\air." + yr + ".nc";
                    break;
                case 3:
                    location = ncLocation + @"slp\slp." + yr + ".nc";
                    break;
            }

    
            DataSet ds = DataSet.Open(location);
            Single[, , ,] res = ds.GetData<Single[, , ,]>(5);
            Double[] time = null;
            if (comboBox1.SelectedIndex == 3) time = ds.GetData<Double[]>(3);
            else time = ds.GetData<Double[]>(4);
            lday = time.Length;
            int diff = lday - fday;

            if (comboBox1.SelectedIndex == 0 | comboBox1.SelectedIndex == 3) lday = lday / 4;

            for (j = 1; j <= 12; j++)
            {

                if (lday <= DateTime.DaysInMonth(yr, j))
                {
                    mon = j;
                    dd = lday;
                    break;
                }
                else lday -= DateTime.DaysInMonth(yr, j);
            }

            DateTime d1 = new DateTime(yr, 1, 1);
      
            Single[, , ,] res2 = new Single[diff, 17, 30, 144];

            time = new Double[diff];
            double[,] timeTicks = new double[diff, 2];

            for (k = fday; k < time.Length; k++) //
            {
                time[k] = d1.DayOfYear;
 
                d1 = d1.AddDays(1);
               
                for (lvl = 0; lvl < 17; lvl++)
                {
                    for (dov = 0; dov < 144; dov++)
                    {
                        for (sh = 0; sh < 30; sh++)
                        {
                            res2[k, lvl, sh, dov] = res[k, lvl, sh, dov];
                        }
                    }
                }
            }

            res = null;

            ds.Dispose();
            File.Delete(location);
            DataSet ds2 = DataSet.Open(location);
            ds2.Add<Int32[]>("level", level, new string[] { dims[1] });

            ds2.Add<Single[]>("lon", lon, new string[] { dims[2] });
            ds2.Add<Single[]>("lat", lat, new string[] { dims[3] });
            ds2.Add<Double[]>("time", time, new string[] { dims[0] });
            ds2.Add<Single[, , ,]>("hgt", res2, new string[] { dims[0], dims[1], dims[3], dims[2] });

            ds2.PutAttr(3, "actual_range", new double[] { 0.0, 357.5 });
            ds2.PutAttr(4, "actual_range", new double[] { 90, 17.5 });
            ds2.PutAttr(5, "valid_range", new int[] { -700, 35000 });
            ds2.Dispose();

            GC.Collect();
            
        }

        private void cbLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Level = cbLevel.SelectedItem as level;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkLastDate();
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkLastDate();
        }



    }
}
