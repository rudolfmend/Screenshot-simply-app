   M S B A M L   `   `   `  ���� N  JScreenshot_2_WpfApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Screenshot_2_WpfAppScreenshot_2_WpfApp  $    Screenshot_2_WpfApp.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:Screenshot_2_WpfApp     -     �Name$  FirstWindow�� � Title$ Screenshot simply app��$	��470��$	��360��
������� � Icon$ /ScreenshotApp.png��.��� -    �Name$ MainGrid�� � Cursor$ Arrow��$��Stretch=�$��Stretch=� � ColumnDefinitions �� $	��27* ��� $	��77* ��� $	��86* ��� $
��170* �.+��� -    ��Name$ ButtonsStackPanel��$��0q�
������� J� Opacity$ 1Q�$��4��.+�o� 
������ $
��Left=�$	��300��.@�m� $��	About App��$��18(�
���   �$��
0 0 0 1.25q�$
��auto��$
��auto��.@��� m� -    m�Name$ AboutMenuItem��$��	About App���� -   	 ��Name$	 CaptureButton��$��Capture Screenshot��$
 Hand�����	#FFB370FF$��24(�$��80��$��15,70,15,15q����� 
 �� 
TargetType"
 ��.���  �� Property � �� �� Value ��  �� 
TargetType" ��.R���  ��Name$ border�� �� CornerRadius$ 10��#��z��#��z�� J� RenderTransformOrigin$ 0.5,0.5&�z���  �� ScaleX$ 1Q� �� ScaleY$ 1Q�._��� $��Center=�$��Center=� �� Triggers P�  P� Property � �� P� Value$
 True�� N� EnterActions �� .���� .��W�  �� 
TargetName$ border�� �� TargetProperty$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)�	 W� To 0.85 g� Duration$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 0.85$ 0:0:0.1D� N� ExitActions �� .���� .��W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)� 1$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 1$ 0:0:0.1D��� -   $	 SettingsButton��$��Settings��$
 Hand�����	#FFC7DBFB$��20(�$��60��$��40,70,40,10q����� "
 ��.���  � �� �� " ��.R��� $ border��$ 10��#��z��#��z��$ 0.5,0.5&�z��� $ 1Q�$ 1Q�._��� $��Center=�$��Center=� P�  � ��$
 True�� �� .���� .��W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)� 0.85$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 0.85$ 0:0:0.1D� �� .���� .��W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)� 1$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 1$ 0:0:0.1D��� -   $	 OpenFolderButton��$��Open Folder��$
 Hand�����#ffd58c$��20(�$��
40,0,40,40q�$��60������ "
 ��.���  � �� �� " ��.R��� $ border��$ 10��#��z��#��z��$ 0.5,0.5&�z��� $ 1Q�$ 1Q�._��� $��Center=�$��Center=� P�  � ��$
 True�� �� .���� .��W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)� 0.85$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 0.85$ 0:0:0.1D� �� .���� .��W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleX)� 1$ 0:0:0.1D�W� $ border��$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)� 1$ 0:0:0.1D�