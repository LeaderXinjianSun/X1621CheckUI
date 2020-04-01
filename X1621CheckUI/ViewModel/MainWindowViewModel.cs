using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using SXJLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion
        #region 方法绑定
        public DelegateCommand CheckCommand { get; set; }
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            this.CheckCommand = new DelegateCommand(()=> { CheckCommandExecute(); });
            Init();
        }
        #endregion
        #region 方法绑定函数
        private async void CheckCommandExecute()
        {
            string stm;
            DataSet ds;
            CheckButtonIsEnabled = false;
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
                            }
                            else
                            {
                                if (BoardChecked)
                                {
                                    stm = "SELECT * FROM BODMSG WHERE SCBODBAR = '" + BoardBarcode + "' ORDER BY SIDATE DESC LIMIT 0,10";
                                    AddMessage(stm);
                                    ds = mysql.Select(stm);
                                    DataGrid1ItemsSource = ds.Tables["table0"];
                                }
                                else
                                {
                                    stm = "SELECT * FROM BODLINE";
                                    AddMessage(stm);
                                    ds = mysql.Select(stm);
                                    DataGrid1ItemsSource = ds.Tables["table0"];
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
            #endregion
            AddMessage("软件加载完成");
        }
        #endregion
    }
}
