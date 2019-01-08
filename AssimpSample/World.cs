using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using System.Reflection;
using System.IO;
using SharpGL.SceneGraph.Quadrics;
using System.Drawing;
using AssimpSample;
using System.Drawing.Imaging;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using System.Windows.Threading;

namespace PF1S14_1
{
    public enum TextureFilterMode
    {
        LinearMipmapLinear
    };

    public enum Axis
    {
        X = 0,
        Y,
        Z
    };
    
    public enum Direction
    {
        Forward = 1,
        Backward = -1
    };
    
    class World
    {

        #region Atributi

        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;

        private int m_width = 0;
        private int m_height = 0;

        private AssimpScene candle_scene;
        private AssimpScene plate_scene;

        private float m_sceneDistance = 800;

        private float[] m_spotPosition = { 0, 0.0f, 0, 1.0f };

        private Cube cube;
        private Cylinder cil;
        private Cylinder cylinderLamp;
        private Sphere sphereLamp;
        private Sphere sphereCandle;

        private enum TextureObjects { Zid = 0, Pod};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;        private uint[] m_textures = null;        private TextureFilterMode m_selectedMode = TextureFilterMode.LinearMipmapLinear;
        private string[] m_textureFiles = { "..//..//images//zid.jpg", "..//..//images//pod2.jpg" };

        public OpenGL gl;        public float scaleCandleSphere = 1;        public float rotateCandle = 0;        public float red_diffuse = 1;        public float green_diffuse = 0.4f;        public float blue_diffuse = 0.15f;        public Boolean light0 = true;        public Boolean light1 = true;        private LookAtCamera lookAtCam;
        //Pomocni vektori preko kojih definisemo lookAt funkciju
        private Vertex direction;
        private Vertex right;
        private Vertex up;

        
        public float xx = -90f;
        
        public float yy = 60f;
        
        public float zz = -10f;
        
        public Boolean perspectiveFPS = false;

        public float lookAtCenterX = 0;
        public float lookAtCenterZ = 10000;

        public float xCenter = 0;
        public float zCenter = 0;

        public float xCandle = -90;
        public float yCandle = 32;
        public float zCandle = 40;

        public int axis;
        public int directionSmer;
        
        public Vertex[] points = {
            new Vertex(-90, 60, -20),
            new Vertex(-90, 60, 150),
            new Vertex( 20, 60, 150),
            new Vertex( 20, 60, -150),
            new Vertex( 120, 60, -150)
        };

        public Vertex nextPoint, currentPoint;
        public int index = 0;

        #endregion

        #region Konstruktori

        public World(/*String scenePath, String sceneFileName,*/ int width, int height, OpenGL gl)
        {
            this.plate_scene = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\..\\..\\..\\3D Models\\Plate"), "Plate.3ds", gl);
            this.candle_scene = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\..\\..\\..\\3D Models\\Candle"), "Candle.3DS", gl);
            this.m_width = width;
            this.m_height = height;
            this.gl = gl;
            m_textures = new uint[m_textureCount];
        }

        public World()
        {
        }

        #endregion

        #region Properties

        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }


        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }
        
       

        #endregion

        #region Metode

        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            SetupLighting(gl);
            SetupTexture(gl);

            plate_scene.LoadScene();
            plate_scene.Initialize();
            candle_scene.LoadScene();
            candle_scene.Initialize();
        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            
            gl.Perspective(50, (float)m_width / m_height, 1, 2000);
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        public void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0.0f, 250.0f, 0.0f, 1.0f };
            float[] light0smer = new float[] { 0.0f, -1.0f, 0.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_DIRECTION, light0smer);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            if (light0 == true)
            {
                gl.Enable(OpenGL.GL_LIGHTING);
                gl.Enable(OpenGL.GL_LIGHT0);
            }else
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_LIGHT0);
            }

            float[] light1pos = new float[] { -90.0f, 0.0f, 40.0f, 1.0f };
            float[] light1diffuse = new float[] { red_diffuse, green_diffuse, blue_diffuse, 1.0f };
            float[] light1ambient = new float[] { 1f, 0.4f, 0.15f, 1.0f };
            float[] light1specular = new float[] { 1f, 0.4f, 0.15f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light0smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            if (light1 == true)
            {
                gl.Enable(OpenGL.GL_LIGHTING);
                gl.Enable(OpenGL.GL_LIGHT1);
            }else {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_LIGHT1);
            }

            // Definisemo belu spekularnu komponentu materijala sa jakim odsjajem
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, light0specular);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);

            //Uikljuci color tracking mehanizam
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            // Podesi na koje parametre materijala se odnose pozivi glColor funkcije
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void SetupTexture(OpenGL gl)
        {
            
           gl.Enable(OpenGL.GL_TEXTURE_2D);
           gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);

            foreach (uint textureId in m_textures)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId); 
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_LINEAR_MIPMAP_LINEAR, OpenGL.GL_REPEAT);
            }

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
           for (int i = 0; i < m_textureCount; ++i)
           {
               // Pridruzi teksturu odgovarajucem identifikatoru
               gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
               // Ucitaj sliku i podesi parametre teksture
               Bitmap image = new Bitmap(m_textureFiles[i]);
               // rotiramo sliku zbog koordinantog sistema opengl-a
               image.RotateFlip(RotateFlipType.RotateNoneFlipY);
               Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
               // RGBA format (dozvoljena providnost slike tj. alfa kanal)
               BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
               gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
               gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
               gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
               gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
               gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

               image.UnlockBits(imageData);
               image.Dispose();            }
           
        }

        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();


            //gl.Translate(m_spotPosition[0], m_spotPosition[1], m_spotPosition[2]);

            if (perspectiveFPS == false)
            {
                gl.Translate(0.0f, 0.0f, -m_sceneDistance);
                gl.Rotate(35, 0, 0);
            }
            else
            {
                gl.LookAt(xx, yy, zz, lookAtCenterX , 0, lookAtCenterZ, 0, 1, 0);
            }


            //lamp
            gl.PushMatrix();
            sphereLamp = new Sphere();
            sphereLamp.CreateInContext(gl);
            sphereLamp.Radius = 0.5f;
            gl.Color(Color.Yellow);
            gl.Translate(0, 250, 0);
            gl.Scale(15, 15, 15);
            sphereLamp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
           

            //cylinder
            gl.PushMatrix();
            cil = new Cylinder();
            gl.Translate(-90, 0, 40);
            gl.Rotate(-90f, 0f, 0f);
            gl.Scale(30, 30, 30);
            cil.TopRadius = 0;
            gl.Color(0.1, 0.4, 0.4);
            cil.CreateInContext(gl);
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //cylinder 2
            gl.PushMatrix();
            cil = new Cylinder();
            gl.Translate(120, 0, -150);
            gl.Rotate(-90f, 0f, 0f);
            gl.Scale(20, 30, 30);
            cil.TopRadius = 0;
            gl.Color(0.2, 0.4, 0.7);
            cil.CreateInContext(gl);
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();




            //backing
            gl.PushMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Pod]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-200.0f, 0, 200.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(200.0f, 0, 200.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(200.0f, 0, -200.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-200.0f, 0, -200.0f);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
           
            gl.Vertex(-200.0f, -1, -200.0f);
            gl.Vertex(200.0f, -1, -200.0f);
            gl.Vertex(200.0f, -1, 200.0f);
            gl.Vertex(-200.0f, -1, 200.0f);


            gl.End();
            gl.PopMatrix();

            //right wall
            gl.PushMatrix();
            cube = new Cube();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Zid]);
            gl.Translate(180, 25, -2);
            gl.Scale(5, 25, 182.5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //desni mali zid - desna soba
            gl.PushMatrix();
            cube = new Cube();
            gl.Translate(80, 25, -95);
            gl.Scale(5, 25, 20);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //left wall
            gl.PushMatrix();
            gl.Translate(-180, 25, -2);
            gl.Scale(5, 25, 182.5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //drugi levi - leva sobe
            gl.PushMatrix();
            gl.Translate(-40, 25, 60);
            gl.Scale(5, 25, 70);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //back wall
            gl.PushMatrix();
            gl.Translate(2, 25, 180);
            gl.Scale(182.5, 25, 5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //zadnji - leva soba
            gl.PushMatrix();
            gl.Translate(-110, 25, -5);
            gl.Scale(70, 25, 5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //zadnji - desna soba
            gl.PushMatrix();
            gl.Translate(130, 25, -80);
            gl.Scale(50, 25, 5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //front wall
            gl.PushMatrix();
            gl.Translate(2, 25, -180);
            gl.Scale(182.5, 25, 5);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
            gl.Disable(OpenGL.GL_TEXTURE_2D);



            //candle lamp
            gl.PushMatrix();
            sphereCandle = new Sphere();
            gl.Translate(xCandle, yCandle + 29, zCandle);
            sphereCandle.CreateInContext(gl);
            sphereCandle.Material = new SharpGL.SceneGraph.Assets.Material();
            sphereCandle.Material.Diffuse = Color.Orange;
            sphereCandle.Radius = 0.2f;
            gl.Color(Color.Orange);
            gl.Scale(15 * scaleCandleSphere, 15 * scaleCandleSphere, 15 * scaleCandleSphere);
            sphereCandle.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(xCandle, yCandle - 1, zCandle);
            gl.Scale(0.1, 0.1, 0.1);
            //gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            plate_scene.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(xCandle, yCandle, zCandle);
            gl.Rotate(0, rotateCandle, 0);
            candle_scene.Draw();
            gl.PopMatrix();

           


            
            gl.DrawText(m_width - 250, 100, 1, 0, 0, "Arial bold", 14, "Predmet: Racunarska grafika");
            gl.DrawText(m_width - 250, 80, 1, 0, 0, "Arial bold", 14, "Skolska godina: 2018/19");
            gl.DrawText(m_width - 250, 60, 1, 0, 0, "Arial bold", 14, "Ime: Srecko");
            gl.DrawText(m_width - 250, 40, 1, 0, 0, "Arial bold", 14, "Prezime: Stojic");
            gl.DrawText(m_width - 250, 20, 1, 0, 0, "Arial bold", 14, "Sifra zadatka: 14.1");


            gl.Flush();
        }

        public void Regler()
        {
            DispatcherTimer timerGo = new DispatcherTimer();
            timerGo.Interval = TimeSpan.FromMilliseconds(100);
            timerGo.Tick += new EventHandler(Go);
            timerGo.Start();
        }

        protected void Go(object sender, EventArgs e)
        {
            if (index.Equals(0))
            {
                loadNext();
            } else
            {
                switch (axis)
                {
                    case (int)Axis.X:
                        {
                            if (xCandle != nextPoint.X)
                            {
                                xCandle += directionSmer * 5;
                                xx = xCandle - directionSmer * 70;
                                zz = zCandle;
                            }
                            else
                            {
                                loadNext();
                            }
                            break;
                        }
                    case (int)Axis.Y:
                        {
                            yCandle += directionSmer * 5;
                            yy = yCandle - directionSmer * 70;
                            break;
                        }
                    case (int)Axis.Z:
                        {
                            if (zCandle != nextPoint.Z)
                            {
                                zCandle += directionSmer * 5;
                                xx = xCandle;
                                zz = zCandle - directionSmer * 70;
                            }
                            else
                            {
                                loadNext();
                            }
                            break;
                        }
                }
                
                if(lookAtCenterX != xCenter)
                {
                    lookAtCenterX += 2000 * (lookAtCenterX < xCenter ? 1 : -1);
                }

                if(lookAtCenterZ != zCenter)
                {
                    lookAtCenterZ += 2000 * (lookAtCenterZ < zCenter ? 1 : -1);
                }
            }

        }

        protected void loadNext()
        {
            float target = 10000;
            currentPoint = nextPoint;
            if (index < points.Length)
            {
                nextPoint = points[index++];

                if (currentPoint.X != nextPoint.X)
                {
                    axis = (int)Axis.X;
                    directionSmer = currentPoint.X < nextPoint.X ? (int)Direction.Forward : (int)Direction.Backward;
                    xCenter = target * directionSmer;
                    zCenter = 0;
                    Console.WriteLine("x osa: " + xCenter + " " + zCenter + " " + lookAtCenterX + " " + lookAtCenterZ);
                }
                else if (currentPoint.Y != nextPoint.Y)
                {
                    axis = (int)Axis.Y;
                    directionSmer = currentPoint.Y < nextPoint.Y ? (int)Direction.Forward : (int)Direction.Backward;
                }
                else if (currentPoint.Z != nextPoint.Z)
                {
                    axis = (int)Axis.Z;
                    directionSmer = currentPoint.Z < nextPoint.Z ? (int)Direction.Forward : (int)Direction.Backward;
                    xCenter = 0;
                    zCenter = target * directionSmer;
                    Console.WriteLine("z osa: " + xCenter + " " + zCenter + " " + lookAtCenterX + " " + lookAtCenterZ);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                plate_scene.Dispose();
                candle_scene.Dispose();
                gl.DeleteTextures(m_textureCount, m_textures);
            }
        }



        #endregion

        #region IDisposable Metode

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        ~World()
        {
            this.Dispose(false);
        }
    }
}
