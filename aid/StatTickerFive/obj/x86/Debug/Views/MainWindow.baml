   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5       StatTickerFive%   StatTickerFive.Views.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      -   5        $��Mainwin��5        � Title$  
StatGadget��6    � WindowStyle$
 None=�5   ^   $
��Auto��6�    � AllowsTransparency .   � SizeToContent$ WidthAndHeight=�5	       � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��6=    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�5
   Y   ��5   r   �� *BackgroundColor+ � Topmost 5
      �� *	IsTopMost+ � WindowState 68   �� *
WInState+.   (System.Windows.Interactivity.Interaction	   Triggers	 5      /   )System.Windows.Interactivity.EventTrigger  5      
   	EventName$
 StateChanged��6   .   (System.Windows.Interactivity.TriggerBase   Actions. 5      6   0System.Windows.Interactivity.InvokeCommandAction     Command 6   �� *WinStatChanged+6@   5        5      $
 LocationChanged��6   . 5         6   �� *WinLocationChanged+6D   5        5      $
 Loaded��6   . 5         6   �� *	WinLoad+69   5        5      $
 MouseLeftButtonUp��6   . 5         6   �� *DropCommand+6=   5        5      $
 Closing��6   . 5         6   �� *
WinClosing+6<   5      .��5      � $
��Auto��6+   ��6   �� *TotalGridHeight+ � RowDefinitions 5       �� 5!      $
��Auto �6   6#   �� 5"      $��* �6   6    .+�5%      � $��0��6	   $
��Auto��6   ��6#   �� *MainGridHeight+	 5'        5(      $
 MouseLeftButtonDown��6   . 5)         6   �� *DragCommand+6?   5*       � ColumnDefinitions 5.      �� 5/      $
��Auto �6   6&   �� 50      $
��Auto �6   6&   .+�53      � -   5        $��MainGrid��53      $��0��6
   $
��auto��6*   $
��Auto��67   y�6E   �� *MainCtrlVisibility+.+�54      �� ��6   �� *MyMainControlCollection+6E   55      � 57      -   5        $��TagGrid��57   E   $��1��6
   $
��Auto��68   ��6   �� *
WrapMargin+.+�58      �� ��6   �� *MyTagControlCollection+��59      �� 5:      .R�5;   	   � $��wrap1��6   $	��Top=�6H    � Orientation 6    �� *WrapOrientation+��6`   �� *	WrapWidth+��6|   �� *
WrapHeight+5<   	   5>      5?      5@      � 5B      -   5        $��TagGrid1��5B   S   $��1��6	   $
��Left=�6�   ��6   �� *Wrap2Margin+��66   �� *
Wrap2Width+y�6c   �� *Wrap2Visibility+	 5C        5D      $
 MouseLeftButtonDown��6   . 5E         6   �� *DragCommand+6?   5F      .+�5I      �� ��6   �� *MyTagControlCollection2+��5J      �� 5K      .R�5L      � $��wrap2��6   $	��Top=�6H    6   �� *Wrap2Orientation+��6`   �� *Wrap2Height+5M      5O      5P      5R      5S      