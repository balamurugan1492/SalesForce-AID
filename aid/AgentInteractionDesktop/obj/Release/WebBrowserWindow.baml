   M S B A M L   `   `   `  ����   Agent.Interaction.Desktop0    *Agent.Interaction.Desktop.WebBrowserWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��WebBrowserWin��  � Title$  WebBrowserWindow�� � 
ResizeMode$ 	CanResize=� � WindowStyle$
 None=� � Icon$@ :/Agent.Interaction.Desktop;component/Images/Agent32x32.ico��
������  � AllowsTransparency . $	��300��$	��300��$	��450��$	��830�� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� MergedDictionaries �� Source��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$��8q�$��1q�
�������	 %� IsVirtualizing	 . 
 %� VirtualizationMode$
 	Recycling=����� *MainBorderBrush+._�� 	 . $
 	Recycling=� � RowDefinitions �� $��* ��� $��5 �.+��� -   $��RightSideRect��$��2��
������ $��10�� K� ZIndex$
 1000��$��Right=�$��Stretch=��� -   $��RightbottomSideRect��$��1��
������ $��10��$��5��$
 2200��$��Right=�$��Bottom=��� -   $��BottomSideRect��$��1��
������ $��10��$
 1000��$��Stretch=�$��Bottom=�� 
�������	 . $
 	Recycling=� �� -    �Name$ grdTitle��$
��Auto ��� -   $ 	grdBottom������ *BottomRowHeight+.+�� -   $��	TitleGrid��$��0�� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -	   $��imgSoftIcon��$��15��$��16��$��5,0,30,0q�$
��Left=�$	��Top=�$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� -
   $��1��$��18�����#007EDF$	��Top=�$��0,-1,0,0q�._��  �� $��* ��� $
��Auto �.+�� $��0��$
��Left=����� *TitleBgColor+ �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblTitleStatus��$��0��$��10,0,0,0q�$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�
������ $
��Left=����� *TitleStatusText+�� -   $��lbldummy��$��1��$��3��$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�
������ $
��Left=��� -   $��txtCallStatus��$��1��$��
-0.3,0,5,0q�$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�$��Right=����� *CallTypeStatus+�� $��2��$��60��$��0,0,6,0q�$
��Auto��$��Right=�$	��Top=� �� Orientation$ 
Horizontal=�.+��� -   $��btnMinimize��$��16��$��20��   MinimizeButton#��[  �� -   $��btnMaximize��$��16��$��20��  MaximizeButton#��[ �� -   $��btnExit��$��16��$��20��  CloseButton#��[ � -   $��BottomWindowGrid��$��1��	 . $
 	Recycling=�$��
10,0,10,10q�.+�(  "System.Windows.Controls.WebBrowser  -   $��
webBrowser��$��Stretch=�$��Stretch=�{�.  