using AssetManagementBase;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using DigitalRise.LevelEditor.UI;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Graphics;

namespace DigitalRise.LevelEditor
{
	public class StudioGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private Desktop _desktop = null;
		private MainForm _mainForm;

		public AssetManager AssetManager { get; private set; }

		//		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();

		public Scene Scene
		{
			get => _mainForm.Scene;
			set => _mainForm.Scene = value;
		}

//		public ForwardRenderer Renderer { get => _mainForm.Renderer; }
		private SpriteBatch _spriteBatch;

		public StudioGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800,
				GraphicsProfile = GraphicsProfile.HiDef
			};

			Window.AllowUserResizing = true;
			IsMouseVisible = true;

			if (Configuration.NoFixedStep)
			{
				IsFixedTimeStep = false;
				_graphics.SynchronizeWithVerticalRetrace = false;
			}
		}

		protected override void LoadContent()
		{
			base.LoadContent();

/*			var baseFolder = @"D:\Temp\Nursia\scenes\scene1";
			var assetManager = AssetManager.CreateFileAssetManager(baseFolder);
			var scene = Scene.Load(Path.Combine(baseFolder, @"scene.json"), assetManager);

			_mainForm.Scene = scene;

			_mainForm.BasePath = baseFolder;
			_mainForm.AssetManager = assetManager;

			// Skybox
			var textureSkyBox = assetManager.LoadTextureCube(GraphicsDevice, "../../skybox/SkyBox.dds");
			scene.Skybox = new Skybox(100)
			{
				Texture = textureSkyBox
			};*/

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Services
			Services.AddService<Game>(this);

			AssetManager = AssetManager.CreateFileAssetManager(Path.Combine(Utility.ExecutingAssemblyDirectory, "../../../../../Assets"));
			Services.AddService(AssetManager);

			DefaultAssets.DefaultFont = AssetManager.LoadFontSystem("Fonts/DroidSans.ttf").GetFont(16);

			// UI
			MyraEnvironment.Game = this;
			_mainForm = new MainForm(Services);

			_desktop = new Desktop();
			_desktop.Widgets.Add(_mainForm);

			// Refresh Library
			var assetFolder = Path.Combine(Utility.ExecutingAssemblyDirectory, "Assets");
			ModelStorage.Load(Path.Combine(assetFolder, "models"));
			_mainForm.RefreshLibrary();

			BuildSampleScene();
		}

		private void BuildSampleScene()
		{

		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

//			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

/*			_mainForm._labelCamera.Text = "Camera: " + Scene.Camera.ToString();
			_mainForm._labelFps.Text = "FPS: " + _fpsCounter.FramesPerSecond;
			_mainForm._labelMeshes.Text = "Meshes: " + Renderer.Statistics.MeshesDrawn;*/

			_desktop.Render();

//			_fpsCounter.Draw(gameTime);

			_spriteBatch.Begin();
			_spriteBatch.End();
		}
	}
}