﻿using MNIST_Demo.Views;
using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MNIST_Demo
{
  public sealed partial class MainPage : Page
  {
    private Helper helper = new Helper();

    private MNISTModel ModelGen = new MNISTModel();
    private MNISTModelInput ModelInput = new MNISTModelInput();
    private MNISTModelOutput ModelOutput = new MNISTModelOutput();

    private RenderTargetBitmap renderBitmap = new RenderTargetBitmap();

    public MainPage()
    {
      this.InitializeComponent();

      // Set supported inking device types.
      inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
      inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(
          new Windows.UI.Input.Inking.InkDrawingAttributes()
          {
            Color = Windows.UI.Colors.White,
            Size = new Size(22, 22),
            IgnorePressure = true,
            IgnoreTilt = true,
          }
      );
      LoadModel();
    }

    private async void LoadModel()
    {
      //Load a machine learning model
      StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/MNIST.onnx"));
      ModelGen = await MNISTModel.CreateMNISTModel(modelFile);
    }

    private async void recognizeButton_Click(object sender, RoutedEventArgs e)
    {
      //Bind model input with contents from InkCanvas      
      ModelInput.Input3 = await helper.GetHandWrittenImage(inkGrid);

      //Evaluate the model
      ModelOutput = await ModelGen.EvaluateAsync(ModelInput);

      //Iterate through evaluation output to determine highest probability digit
      float maxProb = 0;
      int maxIndex = 0;
      for (int i = 0; i < 10; i++)
      {
        if (ModelOutput.Plus214_Output_0[i] > maxProb)
        {
          maxIndex = i;
          maxProb = ModelOutput.Plus214_Output_0[i];
        }
      }
      numberLabel.Text = maxIndex.ToString();

    }

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
      inkCanvas.InkPresenter.StrokeContainer.Clear();
      numberLabel.Text = "";
    }

    private void UxOpenTicTacToeButton_Click(object sender, RoutedEventArgs e)
    {
      this.Frame.Navigate(typeof(TicTacToePage));
    }
  }
}