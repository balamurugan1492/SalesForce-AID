   M S B A M L   `   `   `  ����   Pointel.Interaction.Workbin3    -Pointel.Interaction.Workbin.Forms.PopUpWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � AllowsTransparency  .  � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . 
������ $
��Auto��$	��370�� � SizeToContent$ Height=� � 	Resources    NormalButton �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration g� AutoReverse��%�  &	        �� " ��.���  � ��	 �� "
 ��.R��� $��Border��$ 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. # z #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � ��	 �#007edf:� $ Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$ 1X�$
 0.25X�$ 0:0:0.25D� . P�  � �� . . ��� $ Border�� � ��	 �#0061ac.��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
������� J� BitmapEffect F�  F� Opacity$	 0.5Q� F� Softness$	 0.5Q� F� ShadowDepth$ 0Q� F� Color$ #003660��._��  � RowDefinitions �� $��28 ��� .+��� -   $��txtName��$��0��$��Agent Interaction Desktop��$��14(�$��Stretch=�$��Calibri)����#007edf
��������� $��1�����#656d90$	��0.5q�$��5q�._��  �� $��35��$��* ��� $
��Auto �.+��� -   $��lblAlert��$��0��$
��Wrap=�$��Calibri)�$��12(�
���   �$��8q�� $��1��$��Right=�$��0,0,2,5q�.+��� -   $��btnOK��$��23��$��80��$��0,0,10,0q�$��Right=�$��Bottom=�$	��Yes��#��[  �� -   $��	btnCancel��$��23��$��80��$��0,0,5,0q�$��Right=�$��Bottom=�$��No��#��[  