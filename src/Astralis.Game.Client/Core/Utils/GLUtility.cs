using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Utils;

public class GLUtility
{
    public static void CheckError(GL gl)
    {
        var error = (ErrorCode)gl.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}
