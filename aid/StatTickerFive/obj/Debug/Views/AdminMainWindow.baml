   M S B A M L   `   `   `  ����  StatTickerFive$    StatTickerFive.AdminMainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	     � Topmost  .  � WindowStyle$
 None=�5      
������  � WindowStartupLocation$ CenterScreen=�6F    � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5   #    � Title$ 	AdminMain��5   	   $	��300��6   $	��508��6(   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5	   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5
      6  0System.Windows.Interactivity.InvokeCommandAction  	   Command	 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   . 5        	 6$   �� *DeactivateCommand+6L   5        5   
   $ MouseLeftButtonDown��6   . 5        	 6$   �� *	DragCmd+6B   5        5   
   $ Loaded��6   . 5        	 6$   �� *	LoadCmd+6B   5      
 � 	Resources
 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5      %    5      .��5      �� -   5        $��
MainBorder��5   P   $
��0.89q�6   $	��488��6$   $	��206��60   
�������$��-2,0,0,0q�6b   ._�5   
   � $	��490��6   $��-2,0,0,0q�6    � RowDefinitions 5       �� 5!      $	��175 �6    6/   �� 5"      $��* �6    6-   .+�5$      �� $��1��6   $
��Auto��6!   $��	Statadmin��6/   $��	0,2,390,0q�6C   6X   &   System.Windows.Controls.DataGrid  5%      -   5        $��ObjectsDataGrid��5%   (   $��4,4,6,0q�6   $��0��6?   $	��480��6L      AutoGenerateColumns .    CanUserResizeColumns .     CanUserResizeRows .     EnableRowVirtualization .  50      5>      5?      5@      