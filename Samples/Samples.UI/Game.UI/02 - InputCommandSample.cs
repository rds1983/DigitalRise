using DigitalRise.Input;
using DigitalRise.Geometry;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using DigitalRise.UI.Rendering;
using DigitalRise.UI.Controls;

namespace Samples.UI
{
  [Sample( @"This sample shows how to use configurable input commands.",
    @"A sphere is rendered which can be moved using gamepad or keyboard. The color can 
also be changed.",
    2)]
  [Controls(@"Sample
  Use <Left Stick> on gamepad or <W>/<A>/<S>/<D> on keyboard to move sphere.
  Press <A> on gamepad or <Space> on keyboard to change color.")]
  public class InputCommandSample : Sample
  {
    private Vector2 _position;
    private Color _color;
    private SpriteBatch _spriteBatch;

    private readonly IInputCommand _commandChangeColor;
    private readonly IInputCommand _commandMoveHorizontal;
    private readonly IInputCommand _commandMoveVertical;

    private Texture2D _whiteTexture;

    public InputCommandSample()
    {
      _position = new Vector2(600, 300);
      _color = new Color((Vector3)RandomHelper.Random.NextVector3(0, 1));

      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // Create a command which detect presses of gamepad <A> or keyboard <Space>.
      _commandChangeColor = new ConfigurableInputCommand("ChangeColor")
      {
        Description = "Change color.",
        LogicalPlayerIndex = LogicalPlayerIndex.One,
        PrimaryMapping = new InputMapping
        {
          PositiveButton = Buttons.A,
          PressType = PressType.Press,
        },
        SecondaryMapping = new InputMapping
        {
          PositiveKey = Keys.Space,
          PressType = PressType.Press,
        },
      };

      // Create a command which uses the left stick or <A> and <D> to generate 
      // value for horizontal movement..
      _commandMoveHorizontal = new ConfigurableInputCommand("MoveHorizontal")
      {
        Description = "Move horizontally.",
        LogicalPlayerIndex = LogicalPlayerIndex.One,
        PrimaryMapping = new InputMapping
        {
          Axis = DeviceAxis.GamePadStickLeftX,
        },
        SecondaryMapping = new InputMapping
        {
          PositiveDescription = "Move left.",
          NegativeDescription = "Move right.",
          PositiveKey = Keys.D,
          NegativeKey = Keys.A,
        },
        Sensitivity = 0.5f,   // Use quadratic response curve for thumb stick.
      };

      // Create a command which uses the left stick or <W> and <S> to generate 
      // value for vertical movement.
      _commandMoveVertical = new ConfigurableInputCommand("MoveVertical")
      {
        Description = "Move vertically.",
        LogicalPlayerIndex = LogicalPlayerIndex.One,
        PrimaryMapping = new InputMapping
        {
          Axis = DeviceAxis.GamePadStickLeftY,
        },
        SecondaryMapping = new InputMapping
        {
          PositiveDescription = "Move up",
          NegativeDescription = "Move down",
          PositiveKey = Keys.W,
          NegativeKey = Keys.S,
        },
        Sensitivity = 0.5f,   // Use quadratic response curve for thumb stick.
      };

      InputService.Commands.Add(_commandChangeColor);
      InputService.Commands.Add(_commandMoveHorizontal);
      InputService.Commands.Add(_commandMoveVertical);

      _whiteTexture = UIRenderContext.CreateWhiteTexture(GraphicsDevice);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _spriteBatch.Dispose();
        InputService.Commands.Remove(_commandChangeColor);
        InputService.Commands.Remove(_commandMoveHorizontal);
        InputService.Commands.Remove(_commandMoveVertical);
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // Use command values to move sphere.
      _position.X += _commandMoveHorizontal.Value * deltaTime * 300;
      _position.Y -= _commandMoveVertical.Value * deltaTime * 300;

      // Check command value to determine if color should be changed.
      if (_commandChangeColor.Value > 0)
        _color = new Color((Vector3)RandomHelper.Random.NextVector3(0, 1));

      // Draw a sphere.
      _spriteBatch.Draw(_whiteTexture, new Rectangle((int)_position.X - 100, (int)_position.Y - 100, 200, 200), _color);
    }
  }
}
