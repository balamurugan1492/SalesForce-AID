   M S B A M L   `   `   `  ����   StatTickerFive'    !StatTickerFive.Views.SwitchWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -     � Title$  Select Switch��$	��262��$	��300�� � Topmost .  � WindowStyle$
 None=�
������  � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction  	   Command	 �� *ActivatedCommand+  $ Deactivated��.   	 �� *DeactivateCommand+  $ MouseLeftButtonDown��.   	 �� *	DragCmd+  $ Loaded��.   	 �� *SwitchLoadCommand+
 � 	Resources
  �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .���� $
��0.89q�$	��210��$	��202�� �� CornerRadius$ 10,10,10,10��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $	��222��$	��203��.+��� $��-2,-33,-1,161q�.+��� 	 �� X1$ 4��	 �� Y1$ 60��	 �� X2$	 208��	 �� Y2$ 60�����#656d90$��1��$��77��
 �� Top$ -4���� -   $��btnExit��$��18��$��18��   CloseButton#��[   �� Left$	 185��$ 35������ *SelectClosed+�� $��Select Switch��$��28��$
��Left=�$��0,-1,0,0q�$	��Top=�$��13(�$
��Bold"��� -   $��SwitchListBox��$	��115��$
��Left=�$��5,29,0,0q�$	��Top=�$	��198������ *SwitchCollection+�� $��
Select All��$��16��$
��Left=�$��	5,150,0,0q�$	��Top=� d� 	IsChecked$ FalseY�   $ Checked��.   	 �� *SelectAllChecked+  $ 	Unchecked��.   	 �� *SelectAllUnChecked+�� $��Ok��$��23��$
��Left=�$��
31,173,0,0q�$	��Top=�$��75��  NormalButton#��[ ���� *
SwitchOk+�� -   $��Cancel��$��23��$
��Left=�$��112,173,0,0q�$	��Top=�$��75��#��[ ���� *SwitchCancel+