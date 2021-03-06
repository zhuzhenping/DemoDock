﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartQuant;
using SmartQuant.Shared;
using TD.SandDock;

namespace DemoDock
{
    // 注意：将UserControl改成DockWindow后界面设计器将不可用
    // 是否继承于IUpdatableToolWindow看项目的需求
    public partial class DemoTabWindow : DockWindow, IUpdatableToolWindow //  //UserControl
    {
        public DemoTabWindow()
        {
            InitializeComponent();

            base.Control = demoTabControl; // 此句必须要加，不然报空指针错误

            // 与OnInit相关，因为IDE保存时不记录界面的object参数
            base.PersistState = false;

            base.DefaultDockLocation = ContainerDockLocation.Center;
            base.Text = "DemoTab";

            // 与IUpdatableToolWindow配套，表示有定时刷新功能比如说Portfolio
            SmartQuant.Shared.Global.TimerManager.Add(this);
        }

        protected override void OnInit()
        {
            base.OnInit();
            // 与PersistState相关，由IDE恢复时参数会为null
            if (base.Key == null)
            {
                // 啥都不做
            }
            else
            {
                int key = (int)base.Key;

                this.Text = string.Format("DemoTab[{0}]", key);
            }
        }

        protected override void OnClosing(DockControlClosingEventArgs e)
        {   
            // 这里写这么长是因为SmartQuant.Global与SmartQuant.Shared.Global冲突
            // SmartQuant.Global是给策略开发者用
            // SmartQuant.Shared.Global是给界面开发者用
            if (SmartQuant.Shared.Global.Framework.StrategyManager.Status == StrategyStatus.Running)
            {
                // 做成策略没有停止就不能关闭的示例
                e.Cancel = true;
            }
            
            if (e.Cancel)
            {
                // 如果不关闭的话，不清理和保存
                return;
            }
            // 删除定时刷新
            SmartQuant.Shared.Global.TimerManager.Remove(this);

            base.OnClosing(e);
        }

        void IUpdatableToolWindow.Update()
        {
            this.demoTabControl.UpdateGUI();
        }
    }
}
