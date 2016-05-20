using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using InkNoteLite.Model;
using InkNoteLite.Service;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Diagnostics;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace InkNoteLite.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NotePage : Page
    {
        public NotePage()
        {
            this.InitializeComponent();
            NoteList = new ObservableCollection<InkModel>();
        }

        private ObservableCollection<InkModel> NoteList;

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(WorkPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NoteList.Clear();
            List<InkModel> notes = DataService.GetEntities(a => true).OrderByDescending(a => a.ModifiedOn).ToList();
            notes.ForEach(a => NoteList.Add(a));
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            Task task = Delete();
        }

        private async Task Delete()
        {
            MessageDialog dialog = new MessageDialog("此笔记尚未保存，是否继续 ？", "提示");
            UICommand cmdok = new UICommand("是");
            UICommand cmdno = new UICommand("否");
            dialog.Commands.Add(cmdok);
            dialog.Commands.Add(cmdno);
            var result = await dialog.ShowAsync();
            if (result.Label == "是")
            {
                DataService.DeleteAll();
                NoteList.Clear();
            }
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.FirstOrDefault() as InkModel;
            if (item != null)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(WorkPage), item);
            }
        }
    }
}
