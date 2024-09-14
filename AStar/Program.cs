using SFML.Graphics;
using SFML.System;
using SFML.Window;
using AStar;

Vector2i atlasSize = new(49, 22);
Vector2i gridSize = new(32, 32);
float zoom = 0.5f;

Texture tex = new("Resources/tilemap.png");

RenderWindow window = new(new VideoMode(1600, 1280), "A*");
View view = new(new Vector2f(), new Vector2f(1600, 1280) * zoom);
window.SetView(view);

Tilemap tilemap = new(gridSize);

FileMapGenerator mapGenerator = new();
mapGenerator.Generate(tilemap, "Resources/map.png");

AStarResolverBuilder builder = AStarResolverBuilder.Create()
    .WithOrigin(new(25, 12))
    .WithDestination(new(2, 20));
 
AStarResolver resolver = builder.Build(tilemap);

Renderer renderer = new(tilemap, resolver, tex, atlasSize);

window.SetKeyRepeatEnabled(true);
window.Resized += (s, e) => window.SetView(new View(new Vector2f(), new Vector2f(e.Width, e.Height)));
window.Closed += (s, e) => window.Close();

bool isMovingCam = false;
Vector2f lastPos = new();

window.Resized += (s, e) => view.Size = new Vector2f(e.Width, e.Height) * zoom;

window.MouseWheelScrolled += (s, e) =>
{
    float multiplier = Math.Abs(e.Delta);
    float ratio = e.Delta < 0 ? 1.25f * multiplier : 0.8f / multiplier;
    zoom = Math.Clamp(zoom * ratio, 0.05f, 1.0f);
    view.Size = (Vector2f)window.Size * zoom;
};

window.MouseButtonPressed += (s, e) =>
{
    if (e.Button != Mouse.Button.Left)
        return;

    isMovingCam = true;
    lastPos = new Vector2f(e.X, e.Y);
};

window.MouseButtonReleased += (s, e) =>
{
    if (e.Button != Mouse.Button.Left)
        return;

    isMovingCam = false;
};

window.MouseMoved += (s, e) =>
{
    if (!isMovingCam)
        return;

    Vector2f currentPos = new Vector2f(e.X, e.Y);

    Vector2f offset = (lastPos - currentPos) * zoom;

    view.Move(offset);

    lastPos = currentPos;
};

window.KeyPressed += (s, e) =>
{
    if (e.Code == Keyboard.Key.Space)
        resolver.Tick();

    if (e.Code == Keyboard.Key.R)
    {

    }
        tilemap = new(gridSize);
};

while (window.IsOpen)
{
    window.Clear(new Color(34, 20, 20));
    window.DispatchEvents();
    window.SetView(view);

    window.Draw(renderer);

    window.Display();
}
