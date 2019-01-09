using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;

namespace PF1S14_1
{
    public partial class MainWindow : Window
    {
        #region Atributi

        World m_world = null;

        #endregion

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World((int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }


        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }


        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }


        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.T:
                    if (m_world.RotationX > -30)
                    {
                        m_world.RotationX -= 5.0f;
                    }
                    Console.WriteLine(m_world.RotationX);
                        break;
                case Key.G:
                    if (m_world.RotationX < 55)
                    {

                        m_world.RotationX += 5.0f;
                    }
                    Console.WriteLine(m_world.RotationX);
                        break;

                case Key.F: m_world.RotationY -= 5.0f; break;
                case Key.H: m_world.RotationY += 5.0f; break;

                case Key.Add:
                    if (m_world.SceneDistance > 75)
                    {
                        m_world.SceneDistance -= 75.0f;
                    }
                    Console.WriteLine(m_world.SceneDistance);
                    break;
                case Key.Subtract:
                    if (m_world.SceneDistance < 1500)
                    {
                        m_world.SceneDistance += 75.0f; 
                    }
                    break;
                case Key.F5: Close(); break;

                case Key.A:
                    {
                        m_world.xx += 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }
                case Key.D:
                    {
                        m_world.xx -= 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }
                case Key.Q:
                    {
                        m_world.yy += 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }
                case Key.Z:
                    {
                        m_world.yy -= 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }
                case Key.W:
                    {
                        m_world.zz += 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }
                case Key.S:
                    {
                        m_world.zz -= 5;
                        Console.WriteLine((m_world.xx) + " " + (m_world.yy) + " " + (m_world.zz)); break;
                    }

                case Key.X:
                    {
                        m_world.lookAtCenterX -= 1000;
                        Console.WriteLine(m_world.lookAtCenterX); break;
                    }
                case Key.C:
                    {
                        m_world.lookAtCenterX += 1000;
                        Console.WriteLine(m_world.lookAtCenterX); break;
                    }
                case Key.N:
                    {
                        m_world.lookAtCenterZ -= 1000;
                        Console.WriteLine(m_world.lookAtCenterZ); break;
                    }
                case Key.M:
                    {
                        m_world.lookAtCenterZ += 1000;
                        Console.WriteLine(m_world.lookAtCenterZ); break;
                    }
                case Key.P:
                    {
                        m_world.index = 0;
                        m_world.Regler(); break;
                    }
            }
        }

        private void Rotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(m_world != null)
            {
                m_world.rotateCandle = (float)Rotate.Value;
            }
        }

        private void Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.scaleCandleSphere = (float)Scale.Value;
            }
        }

        private void Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.red_diffuse = (float)Red.Value;
            }
            Console.WriteLine(m_world.red_diffuse);
            m_world.SetupLighting(m_world.gl);
        }

        private void Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.green_diffuse = (float)Green.Value;
            }
            m_world.SetupLighting(m_world.gl);
            Console.WriteLine(m_world.green_diffuse);
        }


        private void Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world != null)
            {
                m_world.blue_diffuse = (float)Blue.Value;
            }
            m_world.SetupLighting(m_world.gl);
            Console.WriteLine(m_world.blue_diffuse);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                if (m_world.light0 == true)
                {
                    m_world.light0 = false;
                }else
                {
                    m_world.light0 = true;
                }
                m_world.SetupLighting(m_world.gl);                
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                if (m_world.light1 == true)
                {
                    m_world.light1 = false;
                }
                else
                {
                    m_world.light1 = true;
                }
                m_world.SetupLighting(m_world.gl);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(m_world != null)
            {
                if (m_world.perspectiveFPS == true)
                {
                    m_world.perspectiveFPS = false;
                }
                else
                {
                    m_world.perspectiveFPS = true;
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if(m_world != null)
            {
                m_world.index = 0;
                m_world.Regler();
            }
        }
    }
}
