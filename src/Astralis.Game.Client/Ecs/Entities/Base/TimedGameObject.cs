using Serilog;

namespace Astralis.Game.Client.Ecs.Entities.Base;

public class TimedGameObject : BaseGameObject
{
    private readonly double _time;
    private double _accumulatedTime;


    public TimedGameObject(double time)
    {
        _time = time;
    }

    public virtual void Trigger()
    {
    }

    public override void Update(double deltaTime)
    {
        _accumulatedTime += deltaTime;


        if (_accumulatedTime >= _time)
        {
            _accumulatedTime = 0;
            Trigger();
        }
    }
}
