   M S B A M L   `   `   `  ���� I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity &     StatTickerFive.Views.LoginWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2e$clr-namespace:StatTickerFive.Helpers     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     -   $��MainView��  � Title$  Login�� � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.ico�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=� � AllowsTransparency .  � Left$ 0��
 � Top$ 0��	 � Topmost	 �� *	IsTopMost+���� *BackgroundColor+.  (System.Windows.Interactivity.Interaction
   Triggers
 /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *ActivatedCommand+  $ Deactivated��.    �� *DeactivateCommand+  $ StateChanged��.    �� *WinStateChanged+  $ MouseLeftButtonDown��.    �� *	DragCmd+  $ Closing��.    �� *
WinClosing+ � 	Resources  �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��0q�
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $��0q� � RowDefinitions �� $
��Auto ��� $��30 �.+�� $��5q�$��0�� �� $��50 ��� $
��Auto ��� $��40 �.+�� $��0��
������� � ColumnDefinitions �� $
��Auto ��� $
��Auto �.+��� $��0��$��Right=�$��	100,3,0,5q�$
��Auto��$��40������ *
IconSource+�� $��10,0,0,0q�$��1��$
��Left=� �� Orientation$ Vertical=�.+��� $��18(�$��-5,3,0,0q�$
��Bold"�$��StatTickerFive��$��Calibri)����� *TitleBackground+�� $��	28,-7,0,3q�$��Version 1.0.33��$��Calibri)�$��12(��� $��1�����#656d90$��0.5,0.5,0.5,0q�._��  �� $
��Auto �.+�� $��0�� �� ���� *ErrorRowHeight+�� $��28 ��� ���� *PasswordHeight+�� ���� *	RowHeight+�� $��0 ��� ���� *	RowHeight+�� ���� *	RowHeight+�� ���� *	RowHeight+�� ���� *AppTypeHeight+ �� $
��Auto �$	��120���� $��* �.+��� $ 
Horizontal=�$��0��$��2��
��� ���$��Stretch=�.+��� $��5q�$��20��$��20��$0��*/StatTickerFive;component/Images/Error.png��$	��Top=��� $��0,0,0,0q�$	��300������  �� TextWrapping$
 Wrap=����� *ErrorMessage+�� $��Username��$��0��$��1����  �� 	UndoLimit$ 10��$��1��$��1��$��0��$��Stretch=�$��2q�   controltempTextbox#��[  ���� *
UserName+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ OneWayToSource=�.��
   $ TextChanged��.    �� *TextChanged+.���� $��Password��$��0��$��2��G� $��1��+   %StatTickerFive.Helpers.PasswordHelper   Attach . $��2��$��2��$��Stretch=�$��2q�  controltempPasswordbox#��[    Password �� *
Password+$ PropertyChanged=�$ TwoWay=�
   $ KeyDown��.    �� *TextChanged+�� $��5��$��2q�$��3��$��1��$��23�� �� 
IsEditable .  �� Text �� *ApplicationName+{��� *IsAppNameEnabled+
   $ KeyDown��.    �� *TextChanged+�� $��5��$��Application Name��$��2��$��	1,1,228,0q��� $��0��$��6��$��Hostname���� $��6��$��2q�$��4��$��1��$��23�� .  �� *Host+{��� *IsHostEnabled+
   $ KeyDown��.    �� *TextChanged+�� $��0��$��7��$
��Port���� $��7��$��2q�$��1��$��5��$��23�� .  �� SelectedIndex$ 0�� �� *Port+{��� *IsPortEnabled+
   $ KeyDown��.    �� *TextChanged+�� $��0��$��8��$��Application Type���� $��8��$��2q�$��1��$��5��$��23�� .   �� *SelectedType+$ TwoWay=�$ PropertyChanged=� �� *SelectedAppType+
   $ KeyDown��.    �� *TextChanged+  $ SelectionChanged��.    �� *AppTypeChanged+.@��� $��
StatServer�� �� 
IsSelected . �� $��Database���� $	��All���� $��0��$��3��$��Place���� $ 10��$��3��$��1��$��2��$��Stretch=�$��2q�#��[  ���� *Place+$ PropertyChanged=�$ TwoWay=�{��� *isPlaceEnabled+.��
   $ TextChanged��.    �� *TextChanged+.���� $��2�����#656d90$	��0.5q�._��� 
�������$	��0.2q�$��1q�._�� $��Center=� �� $
��Auto �.+�� $��1�� �� $
��Auto ��� $��* ��� $��15 ��� $
��Auto �.+�9�   StretchyExpanderTemp#��[ $��0��$��20������ *ExpanderHeader+
   $ Expanded��.    �� *ExpanderExpanded+  $ 	Collapsed��.    �� *ExpanderCollapsed+�� $��23��$��80��$��Right=�$��6��$��Login��$��1��  NormalButton#��[  �� 	IsDefault .   � ForceCursor  . ���� *
LoginClick+�� $��23��$��80��$��Right=�$��7��$��0,0,5,0q�$��3��$��Cancel��#��[ ! �� IsCancel! . ���� *CancelClick+�� $��31��$��Stretch=�$	��Top=�$��1��$��0q����� *TitleBackground+._��� $��© 2013-2016 Pointel Inc��$
��Left=�$��Center=�$��Calibri)�$��12(�$��5,0,0,0q����� *TitleForeground+