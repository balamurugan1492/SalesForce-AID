   M S B A M L   `   `   `  ����   Pointel.Interactions.Email1    +Pointel.Interactions.Email.Forms.MessageBox   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  
MessageBox�� � AllowsTransparency .  � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . 
������ $
��Auto��$	��370�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries	 �� Source��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��8q�
�������._�� 
 � RowDefinitions
 �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblTitle��$��0��$��14(�$��Stretch=�$��Calibri)����#007edf
��������� $��1��$��2�����#656d90$	��0.5q�$��5q�� $��1��$��50��$��10q�
 �� $��35��$��* ��� -    �Name$ growFwd��$��0 �.+��� -   $��txtblockContent��$��0��$
��Wrap=�$��Calibri)�$��12(�
���   �$��8q��� $��1�� �� Orientation$ 
Horizontal=�.+��� -   $��
lblForward��$��50,0,0,0q�$��Forward���� -   $��ForwardError��$��2��
���  ��$��Calibri)�$	��Top=�$��10,2,0,0q�$
��Left=��� -   $��	btn_right��$��	0,0,10,10q�$��2��$��Right=�$��23��$��80��   NormalButton#��[  �� -	   $��btn_left��$��
0,0,100,10q�$��2��$��Right=�$��23��$��80��#��[  