using System;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace Neat.Shaders;

public abstract class ShaderWrapper : ShaderEffect
{
    protected abstract string shaderRelativePath { get; }

    protected ShaderWrapper()
    {
        PixelShader = new PixelShader { UriSource = PackUri(shaderRelativePath) };
    }
    
    protected Uri PackUri(string relativeFile)
    {
        var asm = typeof(ShaderWrapper).Assembly.ToString().Split(',')[0];
        return new Uri("pack://application:,,,/" + asm + ";component/" + relativeFile);
    }

    protected static DependencyProperty RegisterShaderConstantProperty<T>(string propertyName, T defaultValue, Type holderType, int register)
    {
        return DependencyProperty.Register(propertyName, typeof(T), holderType,
            new UIPropertyMetadata(defaultValue, PixelShaderConstantCallback(register)));
    }
    
    protected DispatcherTimer InjectTimer(TimeSpan tick)
    {
        var timer = new DispatcherTimer();
        timer.Interval = tick;
        timer.Start();

        return timer;
    }
}