   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity   StatTickerFive%   StatTickerFive.Views.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     -   $��Mainwin��  � Title$  
StatGadget�� � WindowStyle$
 None=�$
��Auto�� � AllowsTransparency .   � SizeToContent$ WidthAndHeight=� � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png�� � ShowInTaskbar .   � 
ResizeMode$ NoResize=����� *BackgroundColor+ � Topmost �� *	IsTopMost+ � WindowState �� *
WInState+.   (System.Windows.Interactivity.Interaction	   Triggers	 /   )System.Windows.Interactivity.EventTrigger  
   	EventName$
 StateChanged��.   (System.Windows.Interactivity.TriggerBase   Actions. 6   0System.Windows.Interactivity.InvokeCommandAction     Command �� *WinStatChanged+  $
 LocationChanged��.    �� *WinLocationChanged+  $
 Loaded��.    �� *	WinLoad+  $
 MouseLeftButtonUp��.    �� *DropCommand+  $
 Closing��.    �� *
WinClosing+.��� $
��Auto������ *TotalGridHeight+ � RowDefinitions �� $
��Auto ��� $��* �.+�� $��0��$
��Auto������ *MainGridHeight+	   $
 MouseLeftButtonDown��.    �� *DragCommand+ � ColumnDefinitions �� $
��Auto ��� $
��Auto �.+�� -   $��MainGrid��$��0��$
��auto��$
��Auto��y��� *MainCtrlVisibility+.+��� ���� *MyMainControlCollection+� -   $��TagGrid��$��1��$
��Auto������ *
WrapMargin+.+��� ���� *MyTagControlCollection+���� .R�� $��wrap1��$	��Top=� � Orientation �� *WrapOrientation+���� *	WrapWidth+���� *
WrapHeight+� -   $��TagGrid1��$��1��$
��Left=����� *Wrap2Margin+���� *
Wrap2Width+y��� *Wrap2Visibility+	   $
 MouseLeftButtonDown��.    �� *DragCommand+.+��� ���� *MyTagControlCollection2+���� .R�� $��wrap2��$	��Top=� �� *Wrap2Orientation+���� *Wrap2Height+