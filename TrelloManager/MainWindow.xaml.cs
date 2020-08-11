using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;
using Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrelloManager;

namespace TrelloManagerv2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<TrelloLabel> listOfLabels = new ObservableCollection<TrelloLabel>();
        ObservableCollection<TrelloBoard> listOfBoards = new ObservableCollection<TrelloBoard>();
        ObservableCollection<TrelloList> listOflists = new ObservableCollection<TrelloList>();
        Dictionary<string, List<TrelloCard>> listOfCards = new Dictionary<string, List<TrelloCard>>();
        ObservableCollection<string> selectedLists = new ObservableCollection<string>();

        string currentBoard = "";
        private int currentIndexSearchBoard;
        private int currentIndexSearchLabel;


        public TrelloBoard[] SearchListBoard { get; set; } = new TrelloBoard[0];
        public TrelloLabel[] SearchListLabel { get; set; } = new TrelloLabel[0];
        public TrelloList[] SearchListList { get; set; } = new TrelloList[0];

        public MainWindow()
        {
            InitializeComponent();

            LoadService.Setup();

            if (SystemParameters.PrimaryScreenWidth >= 2200) this.Width = 2200;
            else if (SystemParameters.PrimaryScreenWidth >= 1440) this.Width = 1440;
            else if (SystemParameters.PrimaryScreenWidth >= 1000) this.Width = 1000;

            ListViewBoard.Items.Clear();
            ListViewLabel.Items.Clear();
            ListViewLists.Items.Clear();

            if (LoadService.Config.Key.Length > 0 && LoadService.Config.Token.Length > 0)
                BtnLoadBoard_Click(this, null);



        }
        public void RefreshLabelList()
        {


            ListViewLabel.ItemsSource = new List<object>();
            ListViewLabel.ItemsSource = listOfLabels;

            //if (SearchList.Length > 0)
            //    ListView.ScrollIntoView(SearchList[currentIndexSearchBoard]);

            //ListView_SelectionChanged(this, null);
        }
        public void RefreshListOfLists()
        {
            Parallel.ForEach(listOflists, (list) =>
            {
                if (listOfCards.ContainsKey(list.Id))
                    list.QtdCards = listOfCards[list.Id].Count;
            });

            ListViewLists.ItemsSource = new List<object>();
            ListViewLists.ItemsSource = listOflists;

        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            if (File.Exists(txtPath.Text))
            {
                var lines = File.ReadAllLines(txtPath.Text);

                foreach (var line in lines)
                {
                    if (line.Length == 0) continue;
                    var notSkippable = new List<string>{
                        "ID Cliente:",
                        "https://trello"
                    };
                    var shouldSkip = true;
                    foreach (var notSkip in notSkippable)
                    {
                        if (line.StartsWith(notSkip, StringComparison.InvariantCultureIgnoreCase))
                        {
                            shouldSkip = false;
                            break;
                        };
                    }
                    if (shouldSkip) continue;

                    list.Add(line);
                }
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = false;
                dialog.Title = "select file";
                CommonFileDialogResult result = dialog.ShowDialog();


                var filename = dialog.FileName;
                File.WriteAllLines(filename, list);
            }

        }

        private void BtnChangePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.Title = "select file";
            CommonFileDialogResult result = dialog.ShowDialog();

            txtPath.Text = dialog.FileName;
        }

        private void CopyLabelId(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnLoadLabels_Click(object sender, RoutedEventArgs e)
        {
            var labels = Task.Run(() => { return Service.LoadLabels(currentBoard); });


            listOfLabels = new ObservableCollection<TrelloLabel>(await labels);

            RefreshLabelList();
        }

        private void BtnLoadBoard_Click(object sender, RoutedEventArgs e)
        {
            var boards = Service.LoadBoards();

            listOfBoards = new ObservableCollection<TrelloBoard>(boards);

            UpdateListViewBoard();
        }
        private async void BtnLoadLists_Click(object sender, RoutedEventArgs e)
        {
            var lists = Service.LoadList(currentBoard);

            listOflists = new ObservableCollection<TrelloList>(lists);

            RefreshListOfLists();

            await Task.Run(() =>
            {

                Parallel.ForEach(lists, (list) =>
                {
                    var cards = Service.LoadCards(list.Id);

                    if (listOfCards.ContainsKey(list.Id))
                        listOfCards.Remove(list.Id);

                    listOfCards.Add(list.Id, cards);
                });


            });


            RefreshListOfLists();


        }

        private void TxtSearchBoard_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentIndexSearchBoard = 0;
            if (string.IsNullOrEmpty(txtSearchBoard.Text))
            {
                SearchListBoard = new TrelloBoard[0];
            }
            else
            {
                var mod = listOfBoards.FirstOrDefault(c => c.Name.Contains(txtSearchBoard.Text));

                SearchListBoard = listOfBoards.Where(c => c.Name.ToLower().Contains(txtSearchBoard.Text.ToLower())).ToArray();

            }

            UpdateListViewBoard();
        }

        private void UpdateListViewBoard()
        {
            if (SearchListBoard.Length > 0)
                foreach (var board in listOfBoards)
                {
                    board.ViewColor = SearchListBoard.Any(s => s.Id == board.Id) ? ViewColors.SearchColor : "";
                }

            ListViewBoard.ItemsSource = new List<object>();
            ListViewBoard.ItemsSource = listOfBoards;

            if (SearchListBoard.Length > 0)
                ListViewBoard.ScrollIntoView(SearchListBoard[currentIndexSearchBoard]);
        }
        private void TxtSearchBoard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(txtSearchBoard.Text)) return;
                if (SearchListBoard.Length > 0)
                {
                    if (currentIndexSearchBoard == SearchListBoard.Length - 1)
                        currentIndexSearchBoard = 0;
                    else
                        currentIndexSearchBoard++;


                    ListViewBoard.ScrollIntoView(SearchListBoard[currentIndexSearchBoard]);
                }
            }
        }

        private void UpdateListViewLabel()
        {
            foreach (var Label in listOfLabels)
            {
                Label.ViewColor = SearchListLabel.Any(s => s.Id == Label.Id) ? ViewColors.SearchColor : "";
            }



            ListViewLabel.ItemsSource = new List<object>();
            ListViewLabel.ItemsSource = listOfLabels;

            if (SearchListLabel.Length > 0)
                ListViewLabel.ScrollIntoView(SearchListLabel[currentIndexSearchLabel]);
        }
        private void TxtSearchLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(txtSearchLabel.Text)) return;
                if (SearchListLabel.Length > 0)
                {
                    if (currentIndexSearchLabel == SearchListLabel.Length - 1)
                        currentIndexSearchLabel = 0;
                    else
                        currentIndexSearchLabel++;


                    ListViewLabel.ScrollIntoView(SearchListLabel[currentIndexSearchLabel]);
                }
            }
        }

        private void UpdateListViewList()
        {
            //foreach (var List in listOfLists)
            //{
            //    List.ViewColor = SearchListList.Any(s => s.Id == List.Id) ? ViewColors.SearchColor : "";
            //}



            //ListViewList.ItemsSource = new List<object>();
            //ListViewList.ItemsSource = listOfLists;

            //if (SearchListList.Length > 0)
            //    ListViewList.ScrollIntoView(SearchListList[currentIndexSearchList]);
        }
        private void TxtSearchList_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Return)
            //{
            //    if (string.IsNullOrEmpty(txtSearchList.Text)) return;
            //    if (SearchListList.Length > 0)
            //    {
            //        if (currentIndexSearchList == SearchListList.Length - 1)
            //            currentIndexSearchList = 0;
            //        else
            //            currentIndexSearchList++;


            //        ListViewList.ScrollIntoView(SearchListList[currentIndexSearchList]);
            //    }
            //}
        }

        private void TxtSearchLabel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CopyId(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSearchList_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SortHeaderClick(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateCheckBox(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            var wua = new UserAccess();

            wua.Show();
        }

        private async void ChangeBoard(object sender, RoutedEventArgs e)
        {
            RadioButton lbi = (sender as RadioButton);

            var board = ((TrelloBoard)lbi.DataContext);
            foreach (var item in listOfBoards.Where(c => c.Active && c.Id != board.Id))
                item.Active = false;

            currentBoard = board.Id;




            UpdateListViewBoard();
            BtnLoadLabels_Click(this, null);
            BtnLoadLists_Click(this, null);

            await Task.Yield();
        }

        private void BtnLoadCards_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument document = new FlowDocument();

            Paragraph paragraph = new Paragraph();


            if (selectedLists.Count > 0)
                foreach (var item in listOfCards.Where(c => selectedLists.Contains(c.Key)).SelectMany(c => c.Value))
                {
                    paragraph.Inlines.Add($" *****************************************************************************************************{Environment.NewLine}");
                    paragraph.Inlines.Add($" {item.ShortUrl}{Environment.NewLine}");
                    paragraph.Inlines.Add($" {item.Desc}{Environment.NewLine}");
                }


            RtbDetail.Document.Blocks.Clear();
            RtbDetail.Document = document;
            document.Blocks.Add(paragraph);

        }

        private async void UpdateCheckBoxList(object sender, RoutedEventArgs e)
        {
            CheckBox lbi = (sender as CheckBox);

            var list = ((TrelloList)lbi.DataContext);

            if (list.Active)
                selectedLists.Add(list.Id);
            else
                selectedLists.Remove(list.Id);

            await Task.Yield();
        }
    }
}
