   M S B A M L   `   `   `  ���� S  OPointel.TaskbarNotifier, Version=5.0.2.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier   Agent.Interaction.Desktop1   +Agent.Interaction.Desktop.BroadCastNotifier   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     6my-http://schemas.microsoft.com/wpf/2008/toolkit  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � 
ResizeMode$  NoResize=� � AllowsTransparency .  � WindowStyle$
 None=�
������  � Title$ BroadCastNotifier��$
��Auto��$	��400�� � SizeToContent$ Height=� � Topmost .  � 	Resources  �� MergedDictionaries �� Source��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��8q�
����������#0070C5._�� 	 � RowDefinitions	 �� $
��Auto ��� �� $
��Auto �.+��� $��0��$��14(�$��Calibri)����#007edf
����������� *MessageType+�� $��1��$��2q����� *BroadCastBackgroundBrush+._�� -   $��grd_Details��
�������$��1,10,1,1q�
 J� Opacity$	
 0.9Q� � ColumnDefinitions �� $��70 ��� .+��� $��Center=�$��Center=�$��50��$��50������ *MessageIconImageSource+�� -   $��Details��$��1��$��5,0,0,0q�$��Center=� �� Orientation$ Vertical=�.+��� $��12(�$��Calibri)�$
��Bold"����� *
Message1+�� $��12(�$��Calibri)����� *
Message2+�� $��12(�$��Calibri)����� *
Message3+�� $��12(�$��Calibri)����� *
Message4+�� -   $��btn_Show��$��2��$��5,5,90,5q�$y�Visible=�   NormalButton#��[  $��Right=�$��23��$��80��$��_Show���� -   $��btn_Dismiss��$��2��$��5q�#��[  $��Right=�$��23��$��80��$��_Dismiss��