using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Core;
using Core.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using TrelloManager;
using Brushes = System.Windows.Media.Brushes;

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
        List<string> unskippable = new List<string>{
            newLine,
            "ID Cliente:",
            "https://trello",
            "CardId:"
        };

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

            //listOfBoards = new ObservableCollection<TrelloBoard>(CachingService.Load<IEnumerable<TrelloBoard>>(CachingType.Board));
            //listOflists = new ObservableCollection<TrelloList>(CachingService.Load<IEnumerable<TrelloList>>(CachingType.List));
            //listOfCards = CachingService.Load<Dictionary<string, List<TrelloCard>>>(CachingType.Card);
            //currentBoard = CachingService.Load<string>(CachingType.CurrentBoard);

            if (LoadService.Config.Key.Length > 0 && LoadService.Config.Token.Length > 0)
                BtnLoadBoard_Click(this, null);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var source = ((GridView)ListViewLists.View).Columns[2].Header as GridViewColumnHeader;
                SortHeaderClick(source, null);

            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);


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
            BtnReloadCards_Click(this, null);
            ListViewLists.ItemsSource = new List<object>();
            ListViewLists.ItemsSource = listOflists;

        }

        private List<string> CollectLines(RichTextBox richText)
        {
            TextRange textRange = new TextRange(
                richText.Document.ContentStart,
                richText.Document.ContentEnd);

            var text = textRange.Text;

            List<string> resultList = new List<string>();

            using (StringReader sr = new StringReader(text))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                        resultList.Add(line);
                    line = sr.ReadLine();
                }
            }

            return resultList;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {

            if (File.Exists(txtPath.Text))
            {

                var lines = File.ReadAllLines(txtPath.Text);

                var list = ProcessSkip(lines, unskippable);
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = false;
                dialog.Title = "select file";
                CommonFileDialogResult result = dialog.ShowDialog();


                var filename = dialog.FileName;
                File.WriteAllLines(filename, list);
            }
        }
        public List<string> ProcessSkip(string[] lines, List<String> keeperList)
        {
            var list = new List<string>();

            foreach (var line in lines)
            {
                if (line.Length == 0) continue;

                var shouldSkip = true;
                foreach (var notSkip in keeperList)
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

            return list;
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

            //CachingService.Save(boards, $"{ CachingType.Board}"));
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
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(txtSearchLabel.Text)) return;
                if (SearchListLabel.Length > 0)
                {
                    if (currentIndexSearchBoard == SearchListBoard.Length - 1)
                        currentIndexSearchBoard = 0;
                    else
                        currentIndexSearchBoard++;


                    ListViewBoard.ScrollIntoView(SearchListBoard[currentIndexSearchBoard]);
                }
            }
        }

        private void TxtSearchLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentIndexSearchLabel = 0;
            if (string.IsNullOrEmpty(txtSearchLabel.Text))
            {
                SearchListLabel = new TrelloLabel[0];
            }
            else
            {
                var mod = listOfLabels.FirstOrDefault(c => c.Name.Contains(txtSearchLabel.Text));

                SearchListLabel = listOfLabels.Where(c => c.Name.ToLower().Contains(txtSearchLabel.Text.ToLower())).ToArray();

            }

            UpdateListViewLabel();
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

        private void WriteOnRtb(List<string> lines)
        {
            FlowDocument document = new FlowDocument();

            Paragraph paragraph = new Paragraph();

            foreach (var line in lines)
            {
                paragraph.Inlines.Add($"{line}{Environment.NewLine}");

            }

            RtbDetail.Document.Blocks.Clear();
            RtbDetail.Document = document;
            document.Blocks.Add(paragraph);
        }
        private void BtnLoadCards_Click(object sender, RoutedEventArgs e)
        {


            var list = new List<string>();

            if (selectedLists.Count > 0)
                foreach (var item in listOfCards.Where(c => selectedLists.Contains(c.Key)).SelectMany(c => c.Value))
                {
                    list.Add(newLine);
                    list.Add($"CardId: {item.Id}");
                    list.Add($"{item.ShortUrl}");
                    list.Add($"{item.Desc}");
                }
            WriteOnRtb(list);

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

        private void BtnRunSkip_Click(object sender, RoutedEventArgs e)
        {
            var lines = CollectLines(RtbDetail);

            WriteOnRtb(ProcessSkip(lines.ToArray(), unskippable));
        }

        private void BtnGetAttachments_Click(object sender, RoutedEventArgs e)
        {
            var lines = CollectLines(RtbDetail);

            var objects = TextToObject(ProcessSkip(lines.ToArray(), new List<string>{
                "ID Cliente:",
                "CardId",
                newLine
            }));

            Parallel.ForEach(objects, (obj) =>
            {
                if (!Directory.Exists("converted")) Directory.CreateDirectory("attachments");

                if (!Directory.Exists(Path.Combine("attachments", obj.Identifier))) Directory.CreateDirectory(Path.Combine("attachments", obj.Identifier));

                var attachments = Service.LoadCardAttachments(obj.CardId);

                Parallel.ForEach(attachments.OrderByDescending(c => c.Date).Take(3), (attachment) =>
                {
                    Service.Download(obj.Identifier, $"{obj.Identifier}_{Guid.NewGuid()}", attachment.Url);
                });
            });


        }
        public class ModelTest
        {
            public string CardId { get; set; }
            public string Identifier { get; set; }
        }

        const string newLine = "*****************************************************************************************************";
        private List<ModelTest> TextToObject(List<string> lines)
        {
            var list = new List<ModelTest>();

            ModelTest current = null;
            foreach (var item in lines)
            {
                if (item == newLine)
                {
                    if (current != null)
                        list.Add(current);
                    current = new ModelTest();
                    continue;
                }
                if (item.Contains("CardId:", StringComparison.InvariantCultureIgnoreCase))
                {
                    current.CardId = item.Replace("CardId:", "").Trim();
                    continue;
                }
                if (item.Contains("ID Cliente:", StringComparison.InvariantCultureIgnoreCase))
                {
                    current.Identifier = item.Replace("ID Cliente:", "").Trim();
                    continue;
                }

            }
            if (current != null)
                list.Add(current);

            return list;
        }

        private void BtnCleanFiles_Click(object sender, RoutedEventArgs e)
        {

            foreach (var file in Directory.GetFiles("attachments", "*.*", new EnumerationOptions() { RecurseSubdirectories = true }))
            {
                try
                {
                    System.Drawing.Image imgInput = System.Drawing.Image.FromFile(file);

                    var sizedImage = ResizeImage(imgInput, 480, 640);



                    if (!Directory.Exists("converted")) Directory.CreateDirectory("converted");

                    sizedImage.Save($"converted/{Path.GetFileNameWithoutExtension(file).Split("_")[0]}.jpg");

                }
                catch (Exception ex)
                {
                    continue;
                }
            }

        }

        public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int maxWidth = 480, int maxHeight = 640)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private void SortHeaderList_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortColList != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortColList).Remove(listViewSortAdornerList);
                ListViewLists.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortColList == column && listViewSortAdornerList.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortColList = column;
            listViewSortAdornerList = new SortAdorner(listViewSortColList, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortColList).Add(listViewSortAdornerList);
            ListViewLists.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private GridViewColumnHeader listViewSortColList = null;
        private SortAdorner listViewSortAdornerList = null;


        public class SortAdorner : Adorner
        {
            private static Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                    (
                        AdornedElement.RenderSize.Width - 15,
                        (AdornedElement.RenderSize.Height - 5) / 2
                    );
                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }

        }

        private void BtnCardToCsv_Click(object sender, RoutedEventArgs e)
        {

            var list = new List<string>();

            list.Add("TrelloUrl;CardId;Clientid");
            if (selectedLists.Count > 0)
                foreach (var item in listOfCards.Where(c => selectedLists.Contains(c.Key)).SelectMany(c => c.Value))
                {
                    var current = string.Empty;
                    current = $"{item.ShortUrl};";
                    current += $"{item.Id};";
                    var lines = item.Desc.Split("\n");

                    foreach (var line in lines)
                    {
                        if (string.IsNullOrEmpty(line)) continue;

                        if (line.Contains("ID Cliente:", StringComparison.InvariantCultureIgnoreCase))
                        {
                            current += line.Replace("ID Cliente:", "").Trim();
                            break;
                        }

                    }

                    list.Add(current);

                }
            WriteOnRtb(list);

        }

        private void BtnReloadCards_Click(object sender, RoutedEventArgs e)
        {
            Parallel.ForEach(listOflists, (list) =>
            {
                if (listOfCards.ContainsKey(list.Id))
                    list.QtdCards = listOfCards[list.Id].Count;
            });
        }
    }
}
