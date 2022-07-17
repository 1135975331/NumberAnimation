using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NumberAnimation;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private double numberReal;
	private double numberDisplayed;

	private double fixedNum = 100;
		
	// private Thread animThread;
	private readonly Action animAction;
	private bool isAnimActionRunning;
	
	private string currentFileName;
	private FileWriteReadMode currentMode;
	private StreamWriter? recordFileStreamWriter;
	private StreamReader? recordFileStreamReader;

	public MainWindow()
	{
		InitializeComponent();

		NumberDisplay.Content = numberReal.ToString(CultureInfo.CurrentCulture);
	
		numberReal = 0;
		numberDisplayed = 0;
		
		currentFileName = "";



		/*animThread = new Thread(AnimateNumber) {
			IsBackground = true
		};*/
			
			
		animAction = AnimateNumber; // = new Action(AnimateNumber);
		isAnimActionRunning = false;
	}

	private void InitializeFileStreamWriterReader(string fileName, FileWriteReadMode writeOrRead)
	{
		if(currentFileName.Equals(fileName) && writeOrRead == FileWriteReadMode.READ) return;
		
		currentFileName = fileName;
		currentMode = writeOrRead;

		switch(writeOrRead) {
			case FileWriteReadMode.WRITE:
				recordFileStreamReader?.Close();  // => if(recordFileStreamReader != null)  recordFileStreamReader.Close();
				recordFileStreamWriter = new StreamWriter(new FileStream($"{fileName}.txt", FileMode.OpenOrCreate)) { AutoFlush = true };
				break;
				
			case FileWriteReadMode.READ:
				recordFileStreamWriter?.Close();
				recordFileStreamReader = new StreamReader(new FileStream($"{fileName}.txt", FileMode.OpenOrCreate));
				break;
			
			default:
				throw new ArgumentOutOfRangeException(nameof(writeOrRead), writeOrRead, null);
		}
	}


	private void FileSaveButton_Click(object sender, RoutedEventArgs e)
		=> FileSave();

	private void FileLoadButton_Click(object sender, RoutedEventArgs e)
	{
		FileLoad();
		
		if(!isAnimActionRunning)
			StartAnimThread();
	}
	
	private void FileSave()
	{
		File.WriteAllText($"{FileNameInput.Text}.txt", string.Empty);
		using var fsw = new StreamWriter(new FileStream($"{FileNameInput.Text}.txt", FileMode.OpenOrCreate)) {AutoFlush = true};
		
		var strBuilder = new StringBuilder();
		strBuilder.Append($"value={numberReal}");
		
		fsw.Write(strBuilder);
		
		
		/*InitializeFileStreamWriterReader(FileNameInput.Text, FileWriteReadMode.WRITE);
		
		var strBuilder = new StringBuilder();
		strBuilder.AppendLine($"value={numberReal}");
		
		recordFileStreamWriter?.Write(strBuilder);
		recordFileStreamWriter?.Close();*/
	}

	private void FileLoad()
	{
		using var fsr = new StreamReader(new FileStream($"{FileNameInput.Text}.txt", FileMode.OpenOrCreate));
		
		string? line;
		if((line = fsr.ReadLine()) == null) { // 없는 파일을 방금 생성하여 안에 내용이 없는 경우
			FileSave();
			FileLoad();
			return;
		}
		
		var value = line.Split('=')[1];
		
		numberReal = double.Parse(value);


		/*InitializeFileStreamWriterReader(FileNameInput.Text, FileWriteReadMode.READ);

		if(recordFileStreamReader?.ReadLine() == null) { // 없는 파일을 방금 생성하여 안에 내용이 없는 경우
			FileSave();
			FileLoad();
			return;
		}

		var line = recordFileStreamReader.ReadLine() ?? "";
		var value = line.Split('=')[1];
		
		numberReal = double.Parse(value);*/
	}


	private void AddFixedNumDigit_Click(object sender, RoutedEventArgs e)
	{
		fixedNum *= 10; 
			
		AddFixedNumButton.Content = $"+{fixedNum.ToString("N0", CultureInfo.InvariantCulture)}";
		SubtractFixedNumButton.Content = (-fixedNum).ToString("N0", CultureInfo.InvariantCulture);
	}

	private void SubtractFixedNumDigit_Click(object sender, RoutedEventArgs e)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if(fixedNum == 1)  return;
			
		fixedNum /= 10; 
			
		AddFixedNumButton.Content = $"+{fixedNum.ToString("N0", CultureInfo.InvariantCulture)}";
		SubtractFixedNumButton.Content = (-fixedNum).ToString("N0", CultureInfo.InvariantCulture);
	}



	private void AddFixedNumButton_Click(object sender, RoutedEventArgs e)
	{
		numberReal += fixedNum;
			
		if(!isAnimActionRunning)
			StartAnimThread();
	}

	private void SubtractFixedNumButton_Click(object sender, RoutedEventArgs e)
	{
		numberReal -= fixedNum;
			
		if(!isAnimActionRunning)
			StartAnimThread();
	}


	private void ResetNumberButton_Click(object sender, RoutedEventArgs e)
	{
		numberReal = 0;
			
		if(!isAnimActionRunning)
			StartAnimThread();
	}

	private void RandomizeNumberButton_Click(object sender, RoutedEventArgs e)
	{
		var minValParsed = double.TryParse(RandomNumMinInput.Text, out var minVal);
		var maxValParsed = double.TryParse(RandomNumMaxInput.Text, out var maxVal);
			
		if(!(minValParsed && maxValParsed))  return;
			
		var rand = new Random();
		var randVal = rand.NextDouble() * (maxVal - minVal) + minVal;
		numberReal = randVal;
			
		if(!isAnimActionRunning)
			StartAnimThread();
	}

	private void ModifyNumber(object sender, RoutedEventArgs e)
	{
		var parsed = double.TryParse(CustomNumberInput.Text, out var parsedVal);
		var buttonSender = sender.ToString()?.Split(": ")[1];
			
		if(buttonSender == null) return;
		if(!parsed) return;
			
		numberReal = buttonSender switch {
			"Set"      => parsedVal,
			"Add"      => numberReal + parsedVal,
			"Subtract" => numberReal - parsedVal,
			"Multiply" => numberReal * parsedVal,
			"Divide"   => numberReal / parsedVal,
			_          => throw new ArgumentOutOfRangeException()
		};

		if(!isAnimActionRunning)
			StartAnimThread();
	}

		
	private void StartAnimThread() 
		=> Dispatcher.Invoke(DispatcherPriority.Normal, animAction);

	private async void AnimateNumber()
	{
		TasksBeforeAnimation();
			
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		while(numberReal != numberDisplayed) {
			//if(numberReal == numberDisplayed) continue;
			
			var delta = GetDelta(numberReal, numberDisplayed);
				
			if(numberReal > numberDisplayed) // 숫자 증가
				numberDisplayed += Math.Max(1, delta);
			else if(numberReal < numberDisplayed) // 숫자 감소
				numberDisplayed -= Math.Max(1, delta);
			
			numberReal = Math.Floor(numberReal);
			numberDisplayed = Math.Floor(numberDisplayed);
			
			NumberDisplay.Content = numberDisplayed.ToString("N0", CultureInfo.InvariantCulture);
			
			//TextBoxAppendTextWithAutoScroll($"numberReal: {numberReal}, numberDisplayed: {numberDisplayed}\n", DebuggingTextBox);
			
			await Task.Delay(Util.MinMax(10, (int)(15 / delta), 50));
		}
		
		TasksAfterAnimation();
	}

	private void TasksBeforeAnimation()
	{
		isAnimActionRunning = true;
		IsAnimActionRunningDebugCheckBox.IsChecked = isAnimActionRunning;
	}

	private void TasksAfterAnimation()
	{
		isAnimActionRunning = false;
		IsAnimActionRunningDebugCheckBox.IsChecked = isAnimActionRunning;
		
		if((bool) IsAutoSaveEnabled.IsChecked!)
			FileSave();
	}
	

	private static double GetDelta(double v1, double v2)
	{
		var valueDelta = Math.Abs(v1 - v2);
		const double VELOCITY = 0.1;
		var deltaResult = valueDelta * VELOCITY;
		return deltaResult;
		/*return deltaResult switch {
			0   => 0,
			< 1 => 1,
			_   => (decimal)deltaResult
		};*/
	}


	private static void TextBoxAppendTextWithAutoScroll(string textData, TextBox textBox)
	{
		textBox.AppendText(textData);
		textBox.ScrollToEnd();
	}
}