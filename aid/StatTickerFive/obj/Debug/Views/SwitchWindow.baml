   M S B A M L   `   `   `  ����  StatTickerFive'    !StatTickerFive.Views.SwitchWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5          � Title$  Select Switch��5   	   $	��262��6   $	��300��6,    � Topmost .  � WindowStyle$
 None=�6G   
������  � WindowStartupLocation$ CenterScreen=�6s    � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5	   {   .  (System.Windows.Interactivity.Interaction   Triggers 5
      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction  	   Command	 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   . 5        	 6$   �� *DeactivateCommand+6L   5        5   
   $ MouseLeftButtonDown��6   . 5        	 6$   �� *	DragCmd+6B   5        5   
   $ Loaded��6   . 5        	 6$   �� *SwitchLoadCommand+6L   5      
 � 	Resources
 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5      %    5      .��5       �� $
��0.89q�6   $	��210��6$   $	��202��60    �� CornerRadius$ 10,10,10,10��6=   
���������6k   �� *BorderBrush+ J� BitmapEffect 6�   �� *ShadowEffect+._�5!   
   � $	��222��6   $	��203��6   .+�5"      �� $��-2,-33,-1,161q�6   .+�5#      �� 	 �� X1$ 4��5$      	 �� Y1$ 60��6   	 �� X2$	 208��5%      	 �� Y2$ 60��6   ���#656d90$��1��5'      $��77��6   
 �� Top$ -4��6&   �� 5(      -   5        $��btnExit��5(      $��18��6(   $��18��63      CloseButton#��[  6?    �� Left$	 185��6d   $ 35��6v   ��6�   �� *SelectClosed+5)      �� 5*      $��Select Switch��6   $��28��6,   $
��Left=�68   $��0,-1,0,0q�6S   $	��Top=�6e   $��13(�6}   $
��Bold"�6�   �� 5+      -   5        $��SwitchListBox��5+   t   $	��115��6   $
��Left=�6#   $��5,29,0,0q�6>   $	��Top=�6P   $	��198��6h   ��6�   �� *SwitchCollection+�� 5,      $��
Select All��6   $��16��6,   $
��Left=�69   $��	5,150,0,0q�6T   $	��Top=�6g    d� 	IsChecked$ FalseY�6    5-        5.      $ Checked��6%   . 5/        	 60   �� *SelectAllChecked+6W   50        51      $ 	Unchecked��6%   . 52        	 60   �� *SelectAllUnChecked+6Y   53      55      �� 56      $��Ok��6   $��23��6"   $
��Left=�6.   $��
31,173,0,0q�6I   $	��Top=�6]   $��75��6u     NormalButton#��[ 6�   ��6�   �� *
SwitchOk+�� 57      -   5        $��Cancel��57      $��23��6&   $
��Left=�62   $��112,173,0,0q�6M   $	��Top=�6b   $��75��6z   #��[ 6�   ��6�   �� *SwitchCancel+6�   58      59      5:      