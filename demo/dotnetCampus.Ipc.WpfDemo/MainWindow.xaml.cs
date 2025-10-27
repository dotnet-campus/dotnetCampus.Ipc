﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetCampus.Cli;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.WpfDemo.View;

using Walterlv.ThreadSwitchingTasks;

namespace dotnetCampus.Ipc.WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            var options = CommandLine.Parse(Environment.GetCommandLineArgs().Skip(1).ToArray()).As<Options>();
            if (!string.IsNullOrEmpty(options.ServerName))
            {
                Debugger.Launch();
                Debugger.Break();

                StartServer(options.ServerName);

                if (!string.IsNullOrEmpty(options.PeerName))
                {
                    ConnectToPeer(options.PeerName);
                }
            }

            Title = $"dotnetCampus.Ipc.WpfDemo PID={Process.GetCurrentProcess().Id}";
        }

        private async void ConnectToPeer(string peerName)
        {
            var peer = await IpcProvider.GetAndConnectToPeerAsync(peerName);
            AddPeer(peer);
        }

        private void ServerPage_OnServerStarting(object? sender, string e)
        {
            StartServer(e);
        }

        private void StartServer(string serverName)
        {
            var ipcProvider = new IpcProvider(serverName);
            ipcProvider.StartServer();
            ipcProvider.PeerConnected += IpcProvider_PeerConnected;
            IpcProvider = ipcProvider;

            Log($"Start Server Name={serverName}");

            ServerPage.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;

            ServerNameTextBox.Text = serverName;
        }


        /* 项目“dotnetCampus.Ipc.WpfDemo (net45)”的未合并的更改
        在此之前:
                private void IpcProvider_PeerConnected(object? sender, PipeCore.Context.PeerConnectedArgs e)
        在此之后:
                private void IpcProvider_PeerConnected(object? sender, PeerConnectedArgs e)
        */

        /* 项目“dotnetCampus.Ipc.WpfDemo (net45)”的未合并的更改
        在此之前:
                private void IpcProvider_PeerConnected(object? sender, Context.EventArgs.PeerConnectedArgs e)
        在此之后:
                private void IpcProvider_PeerConnected(object? sender, PeerConnectedArgs e)
        */
        private void IpcProvider_PeerConnected(object? sender, Context.PeerConnectedArgs e)
        {
            AddPeer(e.Peer);
        }

        private void AddPeer(PeerProxy peer)
        {
            Dispatcher.InvokeAsync(() =>
            {
                Log($"收到 {peer.PeerName} 连接");

                var currentPeer = ConnectedPeerModelList.FirstOrDefault(temp => temp.PeerName == peer.PeerName);
                if (currentPeer != null)
                {
                    currentPeer.Peer.PeerConnectionBroken -= Peer_PeerConnectBroke;
                    ConnectedPeerModelList.Remove(currentPeer);
                }

                ConnectedPeerModelList.Add(new ConnectedPeerModel(peer));

                peer.PeerConnectionBroken += Peer_PeerConnectBroke;
            });
        }

        private void Peer_PeerConnectBroke(object? sender, IPeerConnectionBrokenArgs e)
        {
            var peer = (PeerProxy) sender!;
            Log($"[连接断开] {peer.PeerName}");
        }

        private IpcProvider IpcProvider { set; get; } = null!;

        private async void Log(string message)
        {
            await Dispatcher.ResumeForeground();
            LogTextBox.Text += DateTime.Now.ToString("hh:mm:ss.fff") + " " + message + "\r\n";
            if (LogTextBox.Text.Length > 2000)
            {
                LogTextBox.Text = LogTextBox.Text.Substring(1000);
            }

            LogTextBox.SelectionStart = LogTextBox.Text.Length - 1;
            LogTextBox.SelectionLength = 0;
            LogTextBox.ScrollToEnd();
        }

        private void AddConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var addConnectPage = new AddConnectPage();
            addConnectPage.ServerStarting += (o, args) =>
            {
                var assembly = GetType().Assembly;
                var file = assembly.Location;
                if (System.IO.Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    file = System.IO.Path.GetFileNameWithoutExtension(file) + ".exe";
                }

                Process.Start(file, $"--server-name {args} --peer-name {ServerNameTextBox.Text}");
                Log($"启动对方服务 {args}");
            };
            addConnectPage.ServerConnecting += (o, args) =>
            {
                ConnectToPeer(args);
            };

            MainPanelContentControl.Content = addConnectPage;
        }

        public ObservableCollection<ConnectedPeerModel> ConnectedPeerModelList { get; } =
            new ObservableCollection<ConnectedPeerModel>();

        private void ConnectedPeerListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var connectedPeerModel = (ConnectedPeerModel) e.AddedItems[0]!;
                var charPage = new CharPage()
                {
                    ConnectedPeerModel = connectedPeerModel,
                    ServerName = ServerNameTextBox.Text
                };
                MainPanelContentControl.Content = charPage;
            }
        }
    }
}
