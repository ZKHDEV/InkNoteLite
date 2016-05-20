using InkNoteLite.Model;
using InkNoteLite.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace InkNoteLite.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WorkPage : Page
    {
        public WorkPage()
        {
            this.InitializeComponent();
            InkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch;
        }

        bool CanBack = false;
        InkModel currentNote;

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            e.Cancel = !CanBack;
            if (!CanBack)
            {
                await Quit();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var targetNote = e.Parameter as InkModel;
            if (targetNote != null)
            {
                currentNote = new InkModel();
                currentNote = targetNote;
                Task task = Load(currentNote.Content);
            }
            else
            {
                currentNote = new InkModel();
                currentNote.Title = "新笔记";
            }
        }

        private async Task Load(byte[] buffer)
        {
            using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0UL)))
                {
                    writer.WriteBytes(buffer);
                    await writer.StoreAsync();
                }
                stream.Seek(0UL);
                await InkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);
            }
        }

        private async Task Quit()
        {
            CanBack = false;
            MessageDialog dialog = new MessageDialog("确定返回 ？", "提示");
            UICommand cmdok = new UICommand("是");
            UICommand cmdno = new UICommand("否");
            dialog.Commands.Add(cmdok);
            dialog.Commands.Add(cmdno);
            var result = await dialog.ShowAsync();
            if (result.Label == "是")
            {
                CanBack = true;
                Frame.GoBack();
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            await AddDialog.ShowAsync();
        }

        private async Task<bool> Save()
        {
            if (InkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count <= 0)
            {
                WarnText.Text = "内容不能为空 ！";
                return false;
            }
            string newTitle = TitleBox.Text.Trim();
            List<InkModel> inks = DataService.GetEntities(a => a.Title == newTitle);
            if (inks.Count > 0)
            {
                if (AddDialog.PrimaryButtonText == "覆盖")
                {
                    var ink = inks.First();
                    ink.ModifiedOn = DateTime.Now;
                    ink.Content = await GetInkBytes();
                    DataService.Update(ink);
                    return true;
                }
                WarnText.Text = "该名称已存在，是否覆盖 ？";
                AddDialog.PrimaryButtonText = "覆盖";
                TitleBox.IsReadOnly = true;
                return false;
            }

            //新建
            currentNote.Title = newTitle;
            currentNote.SubTime = DateTime.Now;
            currentNote.ModifiedOn = DateTime.Now;
            currentNote.Content = await GetInkBytes();
            DataService.Add(currentNote);
            return true;
        }

        private async Task<byte[]> GetInkBytes()
        {
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await InkCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
            byte[] buffer = new byte[stream.Size];
            using (DataReader reader = new DataReader(stream.GetInputStreamAt(0UL)))
            {
                await reader.LoadAsync((uint)stream.Size);
                reader.ReadBytes(buffer);
            }
            stream.Dispose();
            return buffer;
        }

        public async Task AlertAsync(string msg)
        {
            MessageDialog dialog = new MessageDialog(msg, "提示");
            await dialog.ShowAsync();
        }

        private async void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog DelDialog = new MessageDialog("确认删除此笔记 ？", "提示");
            UICommand ConfirmCom = new UICommand("确定", new UICommandInvokedHandler(Delete));
            UICommand CancelCom = new UICommand("取消");
            DelDialog.Commands.Add(ConfirmCom);
            DelDialog.Commands.Add(CancelCom);
            await DelDialog.ShowAsync();
        }

        private async void Delete(IUICommand cmd)
        {
            string Title = currentNote.Title;
            var Inks = DataService.GetEntities(a => a.Title == Title);
            if (Inks.Count <= 0)
            {
                await AlertAsync("你未保存过此笔记 ！");
                return;
            }
            var Ink = Inks.First();
            DataService.Delete(Ink.Id);
            CanBack = true;
            Frame.GoBack();
        }

        private void AddDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            AddDialog.PrimaryButtonText = "保存";
            TitleBox.Text = currentNote.Title;
            TitleBox.IsReadOnly = false;
            WarnText.Text = "";
        }

        private async void AddDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (args.Result == ContentDialogResult.Primary)
            {
                if (TitleBox.Text == "")
                {
                    WarnText.Text = "*不能为空";
                    args.Cancel = true;
                    return;
                }
                bool isCheck = await Save();
                args.Cancel = !isCheck;
            }
        }

        private void OnCut(object sender, RoutedEventArgs e)
        {

        }

        private void OnCopy(object sender, RoutedEventArgs e)
        {

        }

        private void OnPaste(object sender, RoutedEventArgs e)
        {

        }

        private void OnPenColorChanged(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
