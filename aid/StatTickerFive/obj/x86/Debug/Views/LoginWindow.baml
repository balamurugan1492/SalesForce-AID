   M S B A M L   `   `   `  ����I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   &     StatTickerFive.Views.LoginWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2e$clr-namespace:StatTickerFive.Helpers     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   -   5        $��MainView��5   8     � Title$  Login��5	   	    � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.ico��65    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�6/    � SizeToContent$ WidthAndHeight=�6B    � 
ResizeMode$ NoResize=�6b    � AllowsTransparency .  � Left$ 0��6�   
 � Top$ 0��6�   	 � Topmost	 5	      �� *	IsTopMost+��5
   x   �� *BackgroundColor+.  (System.Windows.Interactivity.Interaction
   Triggers
 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   . 5         6$   �� *DeactivateCommand+6L   5        5   
   $ StateChanged��6   . 5         6$   �� *WinStateChanged+6J   5        5   
   $ MouseLeftButtonDown��6   . 5         6$   �� *	DragCmd+6B   5        5   
   $ Closing��6   . 5         6$   �� *
WinClosing+6E   5       � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�6%   %    5!      .��5$   
   �� -   5        $��
MainBorder��5$   F   $
��0.89q�6   $��0q�6(   
���������6X   �� *BorderBrush+ J� BitmapEffect 6|   �� *ShadowEffect+._�5%      � $��0q�6    � RowDefinitions 5&      �� 5'      $
��Auto �6$   64   �� 5(      $��30 �6$   62   .+�5*      � $��5q�6   $��0��6"    5+      �� 5,      $��50 �6(   66   �� 5-      $
��Auto �6(   68   �� 5.      $��40 �6(   66   .+�51      � $��0��6   
������� � ColumnDefinitions 52      �� 53      $
��Auto �6/   �� 54      $
��Auto �6/   .+�56      �� $��0��6   $��Right=�6.   $��	100,3,0,5q�6J   $
��Auto��6]   $��40��6j   ��6v   �� *
IconSource+�� 57      $��10,0,0,0q�6%   $��1��67   $
��Left=�6G    �� Orientation$ Vertical=�6b   .+�58      �� $��18(�6    $��-5,3,0,0q�6.   $
��Bold"�6g   $��StatTickerFive��6y   $��Calibri)�6�   ��6@   �� *TitleBackground+6�   �� 59      $��	28,-7,0,3q�6$   $��Version 1.0.33��67   $��Calibri)�6P   $��12(�6e   6v   5:      5;      �� 5<      $��1��6   ���#656d90$��0.5,0.5,0.5,0q�6A   ._�5>      �  5?      �� 5@   "   $
��Auto �60   6@   .+�5B      � $��0��6#    5D   "   �� 5E   &   ��64   �� *ErrorRowHeight+6X   �� 5F   &   $��28 �64   6B   �� 5G   &   ��64   �� *PasswordHeight+6X   �� 5H   &   ��64   �� *	RowHeight+6S   �� 5I   &   $��0 �64   6A   �� 5J   &   ��64   �� *	RowHeight+6S   �� 5K   &   ��64   �� *	RowHeight+6S   �� 5L   &   ��64   �� *	RowHeight+6S   �� 5M   &   ��64   �� *AppTypeHeight+6W    5O   "   �� 5P   &   $
��Auto �67   $	��120��6D   6U   �� 5Q   &   $��* �67   6C   .+�5S   "   �� $ 
Horizontal=�6-   $��0��6G   $��2��6T   
��� ���$��Stretch=�6|   .+�5T   &   �� $��5q�6,   $��20��67   $��20��6B   $0��*/StatTickerFive;component/Images/Error.png��6N   $	��Top=�6�   �� 5U   &   $��0,0,0,0q�6,   $	��300��6=   ��5V   *   �� 5W   .    �� TextWrapping$
 Wrap=�69   ��6M   �� *ErrorMessage+6m   5Y   '   5[   #   �� 5^   "   $��Username��6(   $��0��6;   $��1��6K   6[   �� 5_   "    �� 	UndoLimit$ 10��6*   $��1��6:   $��1��6G   $��0��6W   $��Stretch=�6�   $��2q�6�      controltempTextbox#��[  6�   ��6d   �� *
UserName+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ OneWayToSource=�.��6  
 5`   &     5a   *   $ TextChanged��69   . 5b   .      6D   �� *TextChanged+6f   5c   +   .��5d   >   5e   #   �� 5f   "   $��Password��6(   $��0��6;   $��2��6K   6Z   G� 5g   "   $��1��6.   +   %StatTickerFive.Helpers.PasswordHelper   Attach . $��2��6�   $��2��6�   $��Stretch=�6�   $��2q�6�     controltempPasswordbox#��[ 6     Password 6Z   �� *
Password+$ PropertyChanged=�$ TwoWay=�
 5h   &     5i   *   $ KeyDown��69   . 5j   .      6D   �� *TextChanged+6f   5k   +   5m   #   �� 5n   "   $��5��6+   $��2q�68   $��3��6C   $��1��6P   $��23��6`    �� 
IsEditable .  �� Text 6~   �� *ApplicationName+{�6�   �� *IsAppNameEnabled+
 5o   &     5p   *   $ KeyDown��69   . 5q   .      6D   �� *TextChanged+6f   5r   +   5t   #   �� 5u   "   $��5��6(   $��Application Name��65   $��2��6P   $��	1,1,228,0q�6d   6y   �� 5v   "   $��0��6(   $��6��68   $��Hostname��6E   6Z   �� 5w   "   $��6��6+   $��2q�68   $��4��6C   $��1��6P   $��23��6`    .  6~   �� *Host+{�6�   �� *IsHostEnabled+
 5x   &     5y   *   $ KeyDown��69   . 5z   .      6D   �� *TextChanged+6f   5{   +   5}   #   �� 5   "   $��0��6(   $��7��68   $
��Port��6E   6V   �� 5�   "   $��7��6+   $��2q�68   $��1��6C   $��5��6S   $��23��6`    .  �� SelectedIndex$ 0��6~    6�   �� *Port+{�6�   �� *IsPortEnabled+
 5�   &     5�   *   $ KeyDown��69   . 5�   .      6D   �� *TextChanged+6f   5�   +   5�   #   �� 5�   "   $��0��6(   $��8��68   $��Application Type��6E   6b   �� 5�   "   $��8��6+   $��2q�68   $��1��6C   $��5��6S   $��23��6`    .   6   �� *SelectedType+$ TwoWay=�$ PropertyChanged=� 6�   �� *SelectedAppType+
 5�   &     5�   *   $ KeyDown��69   . 5�   .      6D   �� *TextChanged+6f   5�   +     5�   *   $ SelectionChanged��69   . 5�   .      6D   �� *AppTypeChanged+6i   5�   +   .@�5�   &   �� $��
StatServer��63    �� 
IsSelected . 6H   �� 5�   &   $��Database��63   �� 5�   &   $	��All��63   5�   #   �� 5�   "   $��0��6(   $��3��68   $��Place��6E   6W   �� 5�   "   $ 10��6*   $��3��6:   $��1��6G   $��2��6W   $��Stretch=�6�   $��2q�6�   #��[  6�   ��6d   �� *Place+$ PropertyChanged=�$ TwoWay=�{�6  �� *isPlaceEnabled+.��6)  
 5�   &     5�   *   $ TextChanged��69   . 5�   .      6D   �� *TextChanged+6f   5�   +   .��5�   >   5�   #   5�      5�      5�      �� 5�      $��2��6   ���#656d90$	��0.5q�6@   ._�5�      �� 
�������$	��0.2q�65   $��1q�6B   ._�5�      � $��Center=�6$    5�   "   �� 5�   &   $
��Auto �64   6D   .+�5�   "   � $��1��6'    5�   &   �� 5�   *   $
��Auto �6;   6J   �� 5�   *   $��* �6;   6G   �� 5�   *   $��15 �6;   6H   �� 5�   *   $
��Auto �6;   6J   .+�5�   &   9�   StretchyExpanderTemp#��[ 6/   $��0��6a   $��20��6q   ��6}   �� *ExpanderHeader+
 5�   *     5�   .   $ Expanded��6=   . 5�   2      6H   �� *ExpanderExpanded+6o   5�   /     5�   .   $ 	Collapsed��6=   . 5�   2      6H   �� *ExpanderCollapsed+6p   5�   /   5�   '   �� 5�   &   $��23��6-   $��80��69   $��Right=�6D   $��6��6`   $��Login��6m   $��1��6}     NormalButton#��[ 6�    �� 	IsDefault .   � ForceCursor  . ��6�   �� *
LoginClick+6�   �� 5�   &   $��23��6-   $��80��69   $��Right=�6D   $��7��6`   $��0,0,5,0q�6m   $��3��6~   $��Cancel��6�   #��[ 6�   ! �� IsCancel! . ��6�   �� *CancelClick+6�   5�   #   5�      5�      5�      5�      �� 5�      $��31��6   $��Stretch=�6L   $	��Top=�6j   $��1��6�   $��0q�6�   ��6%   �� *TitleBackground+._�5�      �� $��© 2013-2016 Pointel Inc��6    $
��Left=�6?   $��Center=�6Z   $��Calibri)�6u   $��12(�6�   $��5,0,0,0q�6�   ��6�   �� *TitleForeground+6�   5�      5�      5�      5�      