   M S B A M L   `   `   `  ����   Screenshot_2_WpfApp(    "Screenshot_2_WpfApp.SettingsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Settings��$	��450��$	��500�� � WindowStartupLocation$ CenterOwner=�.��� $��15q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto � � ColumnDefinitions �� $	��150 ��� $��* �.+��� $��Save Location:��$��0��$��0��$��Center=�$��	0,0,10,10q�]� $��0��$��1��$��0,0,0,10q�.+��� -    ��Name$ BrowseButton��$��Browse��$��Right=�$��80��$��10,0,0,0q��� -    ��Name$ PathTextBox�� �� 
IsReadOnly$
 True���� $��File Format:��$��1��$��0��$��Center=�$��	0,0,10,10q��� -    ��Name$ FormatComboBox��$��1��$��1��$��0,0,0,10q�.@��� $	��PNG���� $
��JPEG���� $��JPEG Quality:��$��2��$��0��$��Center=�$��	0,0,10,10q�����  �� 
TargetType" ��	 �� Triggers	 �� 
 �� Binding
 ��  �� ElementName$ FormatComboBox�� �� Path$ SelectedIndex� �� Value 0.a���  �� Property �@J�Opacity �� Value$	 0.5Q�]� $��2��$��1��$��0,0,0,10q�.+��� $��Right=�$��40�� �� TextAlignment$ Right=����� *Value+$ QualitySlider�� �� StringFormat$ {0:0}%������ " ��	 �� 
 �� $ FormatComboBox��$ SelectedIndex� 0.a���  �@J�Opacity$	 0.5Q��� -    ��Name$ QualitySlider�� � Minimum$ 1Q� � Maximum$	 100Q� �� TickFrequency$ 1Q� � SmallChange$ 1Q� � LargeChange$ 10Q� �� IsSnapToTickEnabled$
 True�� � ToolTip _� .���� $7��1Quality setting is only available for JPEG format���� $��File Name Prefix:��$��3��$��0��$��Center=�$��	0,0,10,10q��� -   $ PrefixTextBox��$��3��$��1��$��0,0,0,10q��� $��Startup Behavior:��$��4��$��0��$��Center=�$��	0,0,10,10q��� -    ��Name$ MinimizeCheckBox��$��4��$��1��$��Minimize on startup��$��0,0,0,10q��� $��Sound Effects:��$��5��$��0��$��Center=�$��	0,0,10,10q��� -   $ SoundCheckBox��$��5��$��1��$,��&Play sound when screenshot is captured��$��0,0,0,10q��� $��File Name Preview:��$��6��$��0��$��Center=�$��	0,0,10,10q��� -   $ PreviewTextBox��$��6��$��1��$
 True��$��0,0,0,10q��� $��8��$��0��$��2�� �� Orientation$ 
Horizontal=�$��Right=�.+��� -	   $ 
SaveButton��$
��Save��$��80��$��30��