﻿using CT_MKWII_WPF.Models.GameData;
using CT_MKWII_WPF.Models.RRInfo;
using CT_MKWII_WPF.Services.WiiManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CT_MKWII_WPF.Views.Components
{
    public partial class PlayerStatsComponent : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _playerName;
        public string PlayerName
        {
            get => _playerName;
            set => SetProperty(ref _playerName, value);
        }

        private string _friendCode;
        public string FriendCode
        {
            get => _friendCode;
            set => SetProperty(ref _friendCode, value);
        }

        private string _vr;
        public string VR
        {
            get => _vr;
            set => SetProperty(ref _vr, value);
        }

        private string _br;
        public string BR
        {
            get => _br;
            set => SetProperty(ref _br, value);
        }
        
        private string _topExtraStat;
        public string TopExtraStat
        {
            get => _topExtraStat;
            set => SetProperty(ref _topExtraStat, value);
        }
        
        private string _bottomExtraStat;
        public string BottomExtraStat
        {
            get => _bottomExtraStat;
            set => SetProperty(ref _bottomExtraStat, value);
        }
        
        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set => SetProperty(ref _isOnline, value);
        }
        
        private BitmapImage _miiImage;
        
        public BitmapImage MiiImage
        {
            get => _miiImage;
            set => SetProperty(ref _miiImage, value);
        }
        

        public PlayerStatsComponent()
        {
            InitializeComponent();
            DataContext = this;
        }
        
        
        public async void UpdateStats(User user)
        {
            Console.WriteLine("Updating stats" + user.MiiName);
            PlayerName = user.MiiName;
            FriendCode = user.FriendCode;
            VR = "VR: "+ user.Vr;
            BR = "BR: "+ user.Br;
            BottomExtraStat = "Races Played: " + user.TotalRaceCount;
            TopExtraStat = "Wins: " + user.TotalWinCount;
            MiiImage = user.MiiImage;
            IsOnline = user.IsOnline;
            ViewRoomButton.Visibility = IsOnline ? Visibility.Visible : Visibility.Hidden;

        }

        public void UpdateStats(Friend player)
        {
            PlayerName = player.MiiName;
            FriendCode = player.FriendCode;
            VR = "VR: "+ player.Vr;
            BR = "BR: "+player.Br;
            BottomExtraStat = "Wins: " + player.Wins;
            TopExtraStat = "Losses: " + player.Losses;
            MiiImage = player.MiiImage;
            IsOnline = player.IsOnline;
            ViewRoomButton.Visibility = IsOnline ? Visibility.Visible : Visibility.Hidden;
        }


        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ViewButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
