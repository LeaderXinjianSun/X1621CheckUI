﻿using BingLibrary.hjb;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using SXJLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X1621CheckUI.ViewModel
{
    class MainWindowViewModel: NotificationObject
    {
        #region 绑定属性
        private string messageStr;

        public string MessageStr
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
                this.RaisePropertyChanged("MessageStr");
            }
        }
        private bool productChecked;

        public bool ProductChecked
        {
            get { return productChecked; }
            set
            {
                productChecked = value;
                this.RaisePropertyChanged("ProductChecked");
            }
        }
        private bool boardChecked;

        public bool BoardChecked
        {
            get { return boardChecked; }
            set
            {
                boardChecked = value;
                this.RaisePropertyChanged("BoardChecked");
            }
        }
        private bool lineChecked;

        public bool LineChecked
        {
            get { return lineChecked; }
            set
            {
                lineChecked = value;
                this.RaisePropertyChanged("LineChecked");
            }
        }
        private DataTable dataGrid1ItemsSource;

        public DataTable DataGrid1ItemsSource
        {
            get { return dataGrid1ItemsSource; }
            set
            {
                dataGrid1ItemsSource = value;
                this.RaisePropertyChanged("DataGrid1ItemsSource");
            }
        }
        private string boardBarcode;

        public string BoardBarcode
        {
            get { return boardBarcode; }
            set
            {
                boardBarcode = value;
                this.RaisePropertyChanged("BoardBarcode");
            }
        }
        private bool checkButtonIsEnabled;

        public bool CheckButtonIsEnabled
        {
            get { return checkButtonIsEnabled; }
            set
            {
                checkButtonIsEnabled = value;
                this.RaisePropertyChanged("CheckButtonIsEnabled");
            }
        }
        private string exportButtonVisibility;

        public string ExportButtonVisibility
        {
            get { return exportButtonVisibility; }
            set
            {
                exportButtonVisibility = value;
                this.RaisePropertyChanged("ExportButtonVisibility");
            }
        }


        #endregion
        #region 方法绑定
        public DelegateCommand CheckCommand { get; set; }
        public DelegateCommand ExportCommand { get; set; }
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            this.CheckCommand = new DelegateCommand(()=> { CheckCommandExecute(); });
            this.ExportCommand = new DelegateCommand(() => { ExportCommandExecute(); });
            Init();
        }
        #endregion
        #region 方法绑定函数
        private async void CheckCommandExecute()
        {
            string stm;
            DataSet ds;
            CheckButtonIsEnabled = false;
            ExportButtonVisibility = "Collapsed";
            await Task.Run(()=> {
                if (BoardBarcode != "")
                {
                    try
                    {
                        Mysql mysql = new Mysql();
                        if (mysql.Connect())
                        {
                            if (ProductChecked)
                            {
                                stm = "SELECT * FROM BARBIND WHERE SCBODBAR = '" + BoardBarcode + "' ORDER BY SIDATE DESC LIMIT 0,60";
                                AddMessage(stm);
                                ds = mysql.Select(stm);
                                DataGrid1ItemsSource = ds.Tables["table0"];
                                ExportButtonVisibility = "Visible";
                            }
                            else
                            {
                                if (BoardChecked)
                                {
                                    stm = "SELECT * FROM BODMSG WHERE SCBODBAR = '" + BoardBarcode + "' ORDER BY SIDATE DESC LIMIT 0,10";
                                    AddMessage(stm);
                                    ds = mysql.Select(stm);
                                    DataGrid1ItemsSource = ds.Tables["table0"];
                                    ExportButtonVisibility = "Visible";
                                }
                                else
                                {
                                    stm = "SELECT * FROM BODLINE";
                                    AddMessage(stm);
                                    ds = mysql.Select(stm);
                                    DataGrid1ItemsSource = ds.Tables["table0"];
                                    ExportButtonVisibility = "Visible";
                                }
                            }

                        }
                        else
                        {
                            AddMessage("数据库未连接");
                        }
                        mysql.DisConnect();
                    }
                    catch (Exception ex)
                    {
                        AddMessage(ex.Message);
                    }

                }
                else
                {
                    AddMessage("请输入板条码");
                }
            });
            CheckButtonIsEnabled = true;
        }
        private void ExportCommandExecute()
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                string filePath = Path.Combine(folderPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                bool result = Csvfile.dt2csv(DataGrid1ItemsSource, filePath, filePath,"");
                if (result)
                {
                    AddMessage(filePath + "导出成功");
                    try
                    {
                        Process process1 = new Process();
                        process1.StartInfo.FileName = filePath;
                        process1.StartInfo.Arguments = "";
                        process1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        process1.Start();
                    }
                    catch (Exception ex)
                    {
                        AddMessage(ex.Message);
                    }
                }
                else
                {
                    AddMessage(filePath + "导出失败");
                }
            }
        }
        #endregion
        #region 自定义函数
        private void AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            MessageStr += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        private void Init()
        {
            #region 界面初始化
            MessageStr = "";
            BoardBarcode = "";
            ProductChecked = true;
            BoardChecked = false;
            LineChecked = false;
            CheckButtonIsEnabled = true;
            ExportButtonVisibility = "Collapsed";
            #endregion
            AddMessage("软件加载完成");
        }
        #endregion
    }
}
