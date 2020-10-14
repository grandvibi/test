using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace SimpleGame1
{
    public partial class Form1 : Form
    {
        Random rng = new Random();

        PictureBox[] stars;
        int backgroundspeed;

        int playerSpeed;

        PictureBox[] enemies;
        int[] enemySpeed;
        int numEnemies;

        PictureBox[] munitions;
        int munitionsSpeed;
        int numMunitions;

        int score;
        bool pause;
        bool gameIsOver;
        int j = 0;

        WindowsMediaPlayer laser;
        WindowsMediaPlayer background;
        WindowsMediaPlayer defeat;

        public Form1()
        {
            InitializeComponent();         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pause = false;
            gameIsOver = false;

            score = 0;

            playerSpeed = 8;
            backgroundspeed = 4;
            numEnemies = 12;

            munitionsSpeed = 40;
            numMunitions = 4;

            laser = new WindowsMediaPlayer();
            background = new WindowsMediaPlayer();
            defeat = new WindowsMediaPlayer();

            background.settings.autoStart = false;
            defeat.settings.autoStart = false;
            laser.settings.autoStart = false;

            laser.URL = "music\\laser.mp3";
            background.URL = "music\\background.mp3";
            defeat.URL = "music\\defeat.mp3";

           

            background.settings.setMode("loop", true);
            background.settings.volume = 10;
            laser.settings.volume = 10;
            defeat.settings.volume = 10;



            Cursor.Hide();
            


            // Stars
            stars = new PictureBox[40];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(rng.Next(0, this.ClientRectangle.Width), rng.Next(25, this.ClientRectangle.Height - 25));
                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.White;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }

                this.Controls.Add(stars[i]);
            }

            // Enemies
            enemies = new PictureBox[numEnemies];
            enemySpeed = new int[numEnemies];

            for (int i = 0; i < numEnemies; i++)
            {
                enemies[i] = new PictureBox();
               // enemies[i].BackColor = Color.Black;
                enemies[i].Height = 20*rng.Next(2, 4);
                enemies[i].Width = enemies[i].Height;
                enemies[i].Top = rng.Next(20, this.ClientRectangle.Height - 10 - enemies[i].Height);
                enemies[i].Left = this.ClientRectangle.Width - enemies[i].Height;
                enemies[i].Image = Properties.Resources.asteroid1;
                enemies[i].SizeMode = PictureBoxSizeMode.StretchImage;

                enemySpeed[i] = rng.Next(5, 12);

                Controls.Add(enemies[i]);

            }
            // Munitions
            munitions = new PictureBox[numMunitions];
            for (int i = 0; i < numMunitions; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(40, 8);
                munitions[i].BackColor = Color.Yellow;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                munitions[i].Visible = false;
                this.Controls.Add(munitions[i]);
            }

            background.controls.play();
        }
 
        // Moving stars
        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length / 2; i++)
            {
                stars[i].Left -= backgroundspeed;

                if (stars[i].Left <= 0)
                {
                    stars[i].Left = this.ClientRectangle.Width - stars[i].Height;
                }
            }
            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Left -= backgroundspeed - 2;

                if (stars[i].Left <= 0)
                {
                    stars[i].Left = this.ClientRectangle.Width - stars[i].Height;
                }
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 20)
            {
                Player.Top -= playerSpeed + 3;
            }
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < this.ClientRectangle.Height - Player.Height - 20)
            {
                Player.Top += playerSpeed + 3;
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {

            if (Player.Left < this.ClientRectangle.Width - Player.Width - 30)
            {
                Player.Left += playerSpeed;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Up)
                    UpMoveTimer.Start();
                if (e.KeyCode == Keys.Down)
                    DownMoveTimer.Start();
                if (e.KeyCode == Keys.Left)
                    LeftMoveTimer.Start();
                if (e.KeyCode == Keys.Right)
                    RightMoveTimer.Start();
            }
                
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                UpMoveTimer.Stop();
            if (e.KeyCode == Keys.Down)
                DownMoveTimer.Stop();
            if (e.KeyCode == Keys.Left)
                LeftMoveTimer.Stop();
            if (e.KeyCode == Keys.Right)
            {
                RightMoveTimer.Stop();
                    }


            if (e.KeyCode == Keys.Space)
            {
                if (!gameIsOver)
                {
                    if (pause)
                    {
                        
                        labelGameOver.Visible = false;
                        pictureBoxControls.Visible = false;
                        pause = false;
                        StartTimers();
                      //  MessageBox.Show("game unpaused");
                    }
                    else
                    {
                        labelGameOver.Text = "      PAUSE";
                        labelGameOver.Visible = true;
                        pictureBoxControls.Visible = true;
                        pause = true;
                        StopTimers();
                      //  MessageBox.Show("game paused");
                    }
                }
            }
            
        }

        // enemies move
        private void EnemiesMoveTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                
                // Move the enemies 
                enemies[i].Left -= enemySpeed[i];

                //
                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    GameOver();
                    enemies[i].Location = new Point(enemies[i].Location.X + (enemies[i].Height / 2) - 40, enemies[i].Location.Y + (enemies[i].Height / 2) - 40);
                    enemies[i].Height = 80;
                    enemies[i].Width = enemies[i].Height;
                    enemies[i].Image = Properties.Resources.boom;
                    defeat.settings.volume = 30;
                    defeat.controls.play();
                    background.controls.stop();
                    
                }

                // Enemies off the edge of the screen
                if (enemies[i].Left + enemies[i].Width < 0)
                {
                    // Score
                    score = score + 1;
                    labelScore.Text = "" + score;

                    if (score % 30 == 0)
                    {
                        for (int j = 0; j < numEnemies; j++)
                            enemySpeed[j] += rng.Next(0,3);
                    }
                    // Reseting enemies
                    enemies[i].Height = 20*rng.Next(2, 4);
                    enemies[i].Width = enemies[i].Height;
                    enemies[i].Top = rng.Next(20, this.ClientRectangle.Height - 10 - enemies[i].Height);
                    enemies[i].Left = this.ClientRectangle.Width - enemies[i].Height;
                }
                for (int h = 0; h < numMunitions; h++)
                {
                    if (munitions[h].Bounds.IntersectsWith(enemies[i].Bounds))
                    {
                        munitions[h].Location = new Point(munitions[h].Location.X, this.ClientRectangle.Width + 10);
                        if (enemies[i].Height <= 40)
                        {
                           // defeat.controls.play();
                            enemies[i].Height = 20 * rng.Next(2, 4);
                            enemies[i].Width = enemies[i].Height;
                            enemies[i].Top = rng.Next(20, this.ClientRectangle.Height - 10 - enemies[i].Height);
                            enemies[i].Left = this.ClientRectangle.Width - enemies[i].Height;
                        }

                        enemies[i].Location = new Point(enemies[i].Location.X + 10, enemies[i].Location.Y + 10);
                        enemies[i].Height -= 20;
                        enemies[i].Width = enemies[i].Height;

                        score += 1;
                        labelScore.Text = "" + score;

                    }
                }

            }
        }

        private void GameOver()
        {
            labelGameOver.Text = "GAME  OVER";
            labelGameOver.Visible = true;
            btnRestart.Visible = true;
            btnExit.Visible = true;
            pictureBoxControls.Visible = true;
            gameIsOver = true;
            StopTimers();
            Cursor.Show();
        }

        private void StartTimers()
        {
            MoveBgTimer.Start();
            EnemiesMoveTimer.Start();
            moveMunitionTimer.Start();
            munitionCreate.Start();

        }

        private void StopTimers()
        {
            MoveBgTimer.Stop();
            EnemiesMoveTimer.Stop();
            UpMoveTimer.Stop();
            DownMoveTimer.Stop();
            LeftMoveTimer.Stop();
            RightMoveTimer.Stop();
            moveMunitionTimer.Stop();
            munitionCreate.Stop();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {

            Application.Restart();

           // this.Controls.Clear();
           // InitializeComponent();
           // Form1_Load(null,null);

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void moveMunitionTimer_Tick(object sender, EventArgs e)
        {
                for (int i = 0; i < numMunitions; i++)
                {
                   // munitions[i].Location = new Point(Player.Location.X + 80, Player.Location.Y + 25);
                    if (munitions[i].Left < this.ClientRectangle.Width)
                    {
                        
                      //  munitions[i].Visible = true;
                        munitions[i].Left += munitionsSpeed;

                    }
                    else
                    {
                        munitions[i].Visible = false;
                      //  munitions[i].Location = new Point(Player.Location.X + 80, Player.Location.Y + 25);
                    }
                }
        }


        private void munitionCreate_Tick(object sender, EventArgs e)
        {
            if (j >= numMunitions - 1)
                j = 0;
            munitions[j].Location = new Point(Player.Location.X + 60, Player.Location.Y + 25);
            munitions[j].Visible = true;
            j++;
            laser.controls.play();

        }
    }
}
