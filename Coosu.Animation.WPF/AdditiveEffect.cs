using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Coosu.Animation.WPF
{
    public class BlendEffect : ShaderEffect
    {

        #region Dependency Properties

        public static readonly DependencyProperty BaseProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Base", typeof(BlendEffect), 0);

        public static readonly DependencyProperty BlendProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Blend", typeof(BlendEffect), 1);

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(BlendModes), typeof(BlendEffect),
                new PropertyMetadata(BlendModes.Normal, OnModeChanged));

        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(double), typeof(BlendEffect),
                    new UIPropertyMetadata(0.5, PixelShaderConstantCallback(0)), OnValidateAmount);

        /// <summary>
        /// Brush that acts as the input for the background layer
        /// </summary>
        public Brush Base
        {
            get => (Brush)GetValue(BaseProperty);
            set => SetValue(BaseProperty, value);
        }

        /// <summary>
        /// Brush that acts as the input for the foreground layer
        /// </summary>
        public Brush Blend
        {
            get => (Brush)GetValue(BlendProperty);
            set => SetValue(BlendProperty, value);
        }

        /// <summary>
        /// Intensity of the color blending between 0 and 1.0
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// Blend mode for calculating the result layer
        /// </summary>
        public BlendModes Mode
        {
            get => (BlendModes)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        #endregion

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is BlendEffect effect && e.NewValue is BlendModes modes)) return;
            string shader;
            switch (modes)
            {
                case BlendModes.Multiply:
                    shader = "shader/multiply.ps";
                    break;
                case BlendModes.Normal:
                default:
                    shader = "shader/add.ps";
                    break;
            }

            effect.PixelShader.UriSource = Global.MakePackUri(shader);
        }

        private static bool OnValidateAmount(object value)
        {
            return value is double amount && (amount >= 0 && amount <= 1.0);
        }

        public BlendEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("shader/add.ps")
            };
            Mode = BlendModes.Normal;
            UpdateShaderValue(BaseProperty);
            UpdateShaderValue(BlendProperty);
            UpdateShaderValue(AmountProperty);
        }

    }
}
