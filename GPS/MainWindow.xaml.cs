using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Drawing;
using Point = System.Windows.Point;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Threading;

namespace GPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dijkstra alg = new Dijkstra();//dijkstra
        List<Cross> crosses = new List<Cross>();//lista wszystkich skrzyzowan
        List<int> route = new List<int>();//droga od startu do końca
        Cross tmp = new Cross();//skrzyżowanie pomocnicze
        Bitmap bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");//mapa
        int[] parents;//kolejni rodzice od punktu końcowego do startu
        int startVertex = 100;//start
        int endVertex = 1;//koniec
        System.Windows.Point mouse = new System.Windows.Point();//punkt zaznaczony myszką
        List<Line> roads = new List<Line>();//droga za pomoca linii
        double angle;//kąt linii
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        //konwertery
        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        //porownanie kolorow
        bool Comparator(System.Drawing.Color x, System.Drawing.Color y)
        {
            int tmp = 40;
            if (x.B <= y.B + tmp && x.B >= y.B - tmp &&
                x.G <= y.G + tmp && x.B >= y.B - tmp &&
                x.R <= y.R + tmp && x.R >= y.R - tmp)
                return true;
            return false;
        }
        //kolorowanie 3x3 piksele
        void coloring(Bitmap bitmap, int x, int y, System.Drawing.Color color)
        {
            bitmap.SetPixel(x, y - 1, color);
            bitmap.SetPixel(x, y, color);
            bitmap.SetPixel(x, y + 1, color);
            bitmap.SetPixel(x - 1, y - 1, color);
            bitmap.SetPixel(x - 1, y, color);
            bitmap.SetPixel(x - 1, y + 1, color);
            bitmap.SetPixel(x + 1, y - 1, color);
            bitmap.SetPixel(x + 1, y, color);
            bitmap.SetPixel(x + 1, y + 1, color);
        }


        public MainWindow()
        {
            InitializeComponent();
            //Bitmap bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
            WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
            map.Source = bitmapTmp;
            //dodanie czarnych punktow jako sktzyzowania

            int counter = 0;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if (Comparator(bitmap.GetPixel(i, j), System.Drawing.Color.Black))
                    {                        
                        Cross tmp = new Cross();
                        tmp.x = i;
                        tmp.y = j;
                        tmp.index = counter;
                        counter++;
                        crosses.Add(tmp);
                    }
                }
            }           

            //wczytanie z pliku
            string line;
            List<string> lines = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (line == "")
                    continue;
                lines.Add(line);
            }

            //zapisanie do crosses
            for (int i = 0; i < crosses.Count; i++)
            {
                string[] vs = lines[i].Split('\t');
                if (vs[0] != "")
                {
                    for (int j = 1; j < vs.Length; j++)
                        crosses[i].neighbours.Add(int.Parse(vs[j]));
                }
            }

            //sortowanie
            for (int i = 0; i < crosses.Count; i++)
                crosses[i].neighbours.Sort();

            //dodanie odleglosci
            for(int i = 0; i < crosses.Count; i++)
            {
                for(int j = 0; j < crosses[i].neighbours.Count; j++)
                {
                    crosses[i].distance.Add(Math.Sqrt(Math.Pow((crosses[i].x - crosses[crosses[i].neighbours[j]].x), 2)
                        + Math.Pow((crosses[i].y - crosses[crosses[i].neighbours[j]].y), 2)));
                }
            }

            //utworzenie listyy linii
            roads.Add(line00);
            roads.Add(line01);
            roads.Add(line02);
            roads.Add(line03);
            roads.Add(line04);
            roads.Add(line05);
            roads.Add(line06);
            roads.Add(line07);
            roads.Add(line08);
            roads.Add(line09);
            roads.Add(line10);
            roads.Add(line11);
            roads.Add(line12);
            roads.Add(line13);
            roads.Add(line14);
            roads.Add(line15);
            roads.Add(line16);
            roads.Add(line17);
            roads.Add(line18);






           
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            


        }

        //zapisuje współrzędne kliknięcia
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapSource bitmapTmp = (BitmapSource)map.Source;
            Bitmap bitmap = new Bitmap(BitmapFromSource(bitmapTmp));
            var point = Mouse.GetPosition(map);
            // int x = point.X/MainWindow.ActualWidthProperty
            double x = column.ActualWidth;
            double y = grid.ActualHeight;
            //  if (a.IsEmpty)
            mouse.X = (int)point.X;
            mouse.Y = (int)point.Y;
        }

        //dijkstra od (a,b)   
        List<int> getRoute(int startVertex, int endVertex)
        {
            List<int> result = new List<int>();
            int index = endVertex;
            result.Add(index);
            while (index != startVertex)
            {
                index = parents[index];
                result.Add(index);
            }

            return result;
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        { 
            var graph = new double[crosses.Count, crosses.Count];
            for (int i = 0; i < crosses.Count; i++)
            {
                for (int j = 0; j < crosses.Count; j++)
                {
                    for (int k = 0; k < crosses[i].neighbours.Count; k++)
                    {
                        if (crosses[i].neighbours[k] == crosses[j].index)
                        {
                            graph[i, j] = crosses[i].distance[k];
                            break;
                        }
                        else
                            graph[i, j] = 0;

                    }
                }
            }


            parents = alg.dijkstra(graph, startVertex);
            route = getRoute(startVertex, endVertex);
        }


       

        //pokazanie drogi
        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");

            for (int i = 0; i < route.Count; i++)
            {
                coloring(bitmap, crosses[route[i]].x, crosses[route[i]].y, System.Drawing.Color.BlueViolet);
            }
            coloring(bitmap, crosses[startVertex].x, crosses[startVertex].y, System.Drawing.Color.GreenYellow);
            coloring(bitmap, crosses[endVertex].x, crosses[endVertex].y, System.Drawing.Color.GreenYellow);
            WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
            map.Source = bitmapTmp;
        }



       
        //rysowanie łamanej od a do b
        private void Test3_Click(object sender, RoutedEventArgs e)
        {
            
            for (int i = 0; i < route.Count-1; i++)
            {
                roads[i].X1 = crosses[route[i]].x   * map.ActualWidth / bitmap.Width;
                roads[i].Y1 = crosses[route[i]].y    * map.ActualHeight / bitmap.Height;
                roads[i].X2 = crosses[route[i + 1]].x *map.ActualWidth / bitmap.Width;
                roads[i].Y2 = crosses[route[i + 1]].y * map.ActualHeight / bitmap.Height; 
                roads[i].Stroke = System.Windows.Media.Brushes.DarkGreen;
                roads[i].StrokeThickness = 20;




            }
            
            car.Stroke = System.Windows.Media.Brushes.Black;
            car.StrokeThickness = 5;
            car.Fill = System.Windows.Media.Brushes.Yellow;

            route.Reverse();

            var x1 = crosses[route[0]].x * map.ActualWidth / bitmap.Width;
            var x2 = crosses[route[0]].y * map.ActualHeight / bitmap.Height;
            x1 -= car.Width / 2;
            x2 -= car.Height / 2;

            var place = new Thickness(x1, x2, 0, 0);
            car.Margin = place;
        }

        //ruch pojazdu
        private void Test4_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
 

        }

        int counter = 0;
        int counter2 = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (counter == route.Count - 1)
            {
                dispatcherTimer.Stop();
                return;
            }

            double x0 = crosses[route[counter]].x * map.ActualWidth / (double)bitmap.Width;
            double x1 = crosses[route[counter + 1]].x * map.ActualWidth / (double)bitmap.Width;
            double y0 = crosses[route[counter]].y * map.ActualHeight / (double)bitmap.Height;
            double y1 = crosses[route[counter + 1]].y * map.ActualHeight / (double)bitmap.Height;
            double x = x1 - x0;
            double y = y1 - y0;
            angle = y / x;
            if (y < 0 && x > 0)
                angle = -Math.Abs(angle);
            if (y < 0 && x < 0)
                angle = -Math.Abs(angle);
            if (y > 0 && x < 0)
                angle = Math.Abs(angle);




            var newPlace = car.Margin;
            if (crosses[route[counter + 1]].x < crosses[route[counter]].x)
                newPlace.Left--;
            else
                newPlace.Left++;
            newPlace.Top = newPlace.Top + angle;
            car.Margin = new Thickness(newPlace.Left, newPlace.Top, 0, 0);

            double Y = car.Margin.Top * bitmap.Height / map.ActualHeight;
            double X = car.Margin.Left * bitmap.Width / map.ActualWidth;

            counter2++;
            //double bitmapTmp=
            if (counter2 == (int)(Math.Abs(crosses[route[counter + 1]].x - crosses[route[counter]].x) * map.ActualWidth / bitmap.Width)) 
            {
                counter2 = 0;
                counter++;
            }





            /*
            if ((Math.Abs(car.Margin.Top * bitmap.Height / map.ActualHeight - crosses[route[counter + 1]].y)) < 1)
               // && Math.Abs(car.Margin.Left * bitmap.Width / map.ActualWidth - crosses[route[counter + 1]].x) < 1)
                
            
            counter++;
                */
        }






        //pokazanie wezla i sasiadow
        private void ShowVertex(object sender, TextChangedEventArgs e)
        {
            if (showVertex.Text != "")
            {
                int X = crosses[int.Parse(showVertex.Text)].x;
                int Y = crosses[int.Parse(showVertex.Text)].y;
                bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
                coloring(bitmap, X, Y, System.Drawing.Color.GreenYellow);
                for (int i = 0; i < crosses[int.Parse(showVertex.Text)].neighbours.Count; i++)
                {
                    coloring(bitmap, crosses[crosses[int.Parse(showVertex.Text)].neighbours[i]].x,
                        crosses[crosses[int.Parse(showVertex.Text)].neighbours[i]].y, System.Drawing.Color.Blue);
                }
                WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
                map.Source = bitmapTmp;
            }
        }

        //punkt startowy
        private void StartVertex_TextChanged(object sender, TextChangedEventArgs e)
        {
            startVertex = int.Parse(StartVertex.Text);
        }

        //punkt końcowy
        private void EndVertex_TextChanged(object sender, TextChangedEventArgs e)
        {
            endVertex = int.Parse(EndVertex.Text);
        }



    }
}
