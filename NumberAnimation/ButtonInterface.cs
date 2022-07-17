using System.Windows;

namespace NumberAnimation;

public interface INumberButton
{
	void SubtractFixedNumDigit_Click(object sender, RoutedEventArgs e);

	void AddFixedNumDigit_Click(object sender, RoutedEventArgs e);

	void SubtractFixedNumButton_Click(object sender, RoutedEventArgs e);

	void AddFixedNumButton_Click(object sender, RoutedEventArgs e);

	void ResetNumberButton_Click(object sender, RoutedEventArgs e);

	void SetNumber_Click(object sender, RoutedEventArgs e);

	void AddNumberButton_Click(object sender, RoutedEventArgs e);

	void SubtractNumberButton_Click(object sender, RoutedEventArgs e);

	void MultiplyNumberButton_Click(object sender, RoutedEventArgs e);

	void DivideNumberButton_Click(object sender, RoutedEventArgs e);
}