using System;
using System.Windows;
using System.Windows.Media;

namespace Neat.Shaders;

public class SquareGlowEffect : ShaderWrapper
{
    protected override string shaderRelativePath => "Shaders/SquareGlowShader.ps";
    private float tickCount;

    public SquareGlowEffect()
    {
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(SpeedProperty);
        UpdateShaderValue(SegmentsProperty);
        UpdateShaderValue(AttenuationProperty);
        UpdateShaderValue(SquareSizeProperty);
        UpdateShaderValue(SquareFillProperty);
        UpdateShaderValue(SquareGlowProperty);
        UpdateShaderValue(FinalIntensityProperty);

        var timer = InjectTimer(TimeSpan.FromMilliseconds(3));
        timer.Tick += delegate
        {
            Time = ++tickCount * 0.03f;
        };
    }

    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(SquareGlowEffect), 0);

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public static readonly DependencyProperty TimeProperty = RegisterShaderConstantProperty(nameof(Time), 0f, typeof(SquareGlowEffect), 2);

    public float Time
    {
        get => (float)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public static readonly DependencyProperty SpeedProperty = RegisterShaderConstantProperty(nameof(Speed), 0.1f, typeof(SquareGlowEffect), 3);

    public float Speed
    {
        get => (float)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    public static readonly DependencyProperty SegmentsProperty = RegisterShaderConstantProperty(nameof(Segments), 2f, typeof(SquareGlowEffect), 4);

    public float Segments
    {
        get => (float)GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public static readonly DependencyProperty AttenuationProperty = RegisterShaderConstantProperty(nameof(Attenuation), 1.5f, typeof(SquareGlowEffect), 5);

    public float Attenuation
    {
        get => (float)GetValue(AttenuationProperty);
        set => SetValue(AttenuationProperty, value);
    }

    public static readonly DependencyProperty SquareSizeProperty = RegisterShaderConstantProperty(nameof(SquareSize), 0.2f, typeof(SquareGlowEffect), 6);

    public float SquareSize
    {
        get => (float)GetValue(SquareSizeProperty);
        set => SetValue(SquareSizeProperty, value);
    }

    public static readonly DependencyProperty SquareFillProperty = RegisterShaderConstantProperty(nameof(SquareFill), 0.15f, typeof(SquareGlowEffect), 7);

    public float SquareFill
    {
        get => (float)GetValue(SquareFillProperty);
        set => SetValue(SquareFillProperty, value);
    }

    public static readonly DependencyProperty SquareGlowProperty = RegisterShaderConstantProperty(nameof(SquareGlow), 65f, typeof(SquareGlowEffect), 8);

    public float SquareGlow
    {
        get => (float)GetValue(SquareGlowProperty);
        set => SetValue(SquareGlowProperty, value);
    }

    public static readonly DependencyProperty FinalIntensityProperty = RegisterShaderConstantProperty(nameof(FinalIntensity), 4f, typeof(SquareGlowEffect), 9);

    public float FinalIntensity
    {
        get => (float)GetValue(FinalIntensityProperty);
        set => SetValue(FinalIntensityProperty, value);
    }
}

