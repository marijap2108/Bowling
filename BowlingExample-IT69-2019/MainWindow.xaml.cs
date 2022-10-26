using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace BowlingExample_IT69_2019
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game = new Game();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            for(int i=1; i<11; i++)
            {
                int tn = i == 10 ? 4 : 3;
                // input
                for(int j=1; j<tn; j++)
                {
                    TextBox throwing = (TextBox)this.FindName("f" + i + "t" + j);
                    if (throwing.Text.Length > 0)
                    {
                        int thrown = game.getFrame(i-1).setThrow(j, Convert.ToInt32(throwing.Text));
                        if (thrown == 2)
                        {
                            throwing.Text = "";
                            return;
                        }
                        if (thrown == 1 && j == 1)
                        {
                            TextBox nextThrowing = (TextBox)this.FindName("f" + i + "t" + 2);
                            nextThrowing.Text = "0";
                            nextThrowing.IsReadOnly = true;
                        }
                    }
                }
            }
            for (int i = 1; i < 11; i++)
            {
                // output
                TextBox score = (TextBox)this.FindName("f" + i);
                if (game.getFrame(i - 1).getCompleted())
                {
                    score.Text = game.getScore(i).ToString();
                }
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(!Regex.IsMatch(textBox.Text, "^([0-9]|10)?$|^0$"))
            {
                textBox.Text = "";
            }
        }
    }

    public class Frame
    {
        int firstThrow = 0;
        int secondThrow = 0;
        int thirdThrow = 0;
        bool completed = false;
        int thrown = 0;
        public enum Clear
        {
            none,
            spare,
            strike
        }
        Clear clear = Clear.none;
        Frame nextFrame;

        public void setNextFrame(Frame nextFrame)
        {
            this.nextFrame = nextFrame;
        }

        public bool getCompleted()
        {
            if (nextFrame == null)
            {
                return completed;
            }
            return clear == Clear.none ? completed : nextFrame.getThrown() == 1;
        }

        public int getThrown()
        {
            return thrown;
        }

        private void setClear()
        {
            if (firstThrow == 10)
            {
                clear = Clear.strike;
                completed = false;
                return;
            }
            if (firstThrow + secondThrow == 10)
            {
                clear = Clear.spare;
                completed = false;
                return;
            }
        }

        public Clear getClear()
        {
            return clear;
        }

        public int getScore()
        {
            int score = firstThrow + secondThrow + thirdThrow;
            if (nextFrame != null)
            {
                if (clear == Clear.spare)
                {
                    return score + nextFrame.firstThrow;
                }
                if (clear == Clear.strike)
                {
                    return score + nextFrame.firstThrow + nextFrame.secondThrow;
                }
            }
            return score;
        }

        public int setThrow(int i, int score)
        {
            if (i == 1)
            {
                this.firstThrow = score;
                setClear();
                if (score == 10)
                {
                    return thrown = 1;
                }
                return thrown = 0;
            }
            if(i == 2)
            {
                this.secondThrow = score;     
                if(nextFrame != null)
                {
                    completed = true;
                    setClear();
                    return thrown = secondThrow + firstThrow > 10 ? 2 : 1;
                } else
                {
                    if(firstThrow + secondThrow < 10)
                    {
                        completed = true;
                    }
                }
                setClear();
                return thrown = 0;
            }
            if(i == 3)
            {
                this.thirdThrow = score;
                completed = true;
                return thrown = 1;
            }
            return 0;
        }
    }

    public class Game
    {
        Frame[] frames = new Frame[10];

        public Game()
        {
            for(int i=0; i<10; i++)
            {
                frames[i] = new Frame();
            }
            for (int i = 0; i < 9; i++)
            {
                frames[i].setNextFrame(frames[i+1]);
            }
        }

        public Frame getFrame(int i)
        {
            return frames[i];
        }

        public int getScore(int i)
        {
            int score = 0;
            for(int j=0; j<i; j++)
            {
                score += frames[j].getScore();
            }
            return score;
        }

        public Frame[] getFrames()
        {
            return frames;
        }
    }

}
