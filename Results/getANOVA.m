function getANOVA
    times = csvread('studyTimes.csv');
    assistedTimes = times(:, 1);
    nonAssistedTimes = times(:, 2);
    
    assistanceLevel = {'A' 'N'};
    disp(times(5,:));
    disp(assistanceLevel(1, :));
    
    T = table(assistanceLevel(1,:)', times(1,:)', times(2,:)', times(3,:)', times(4,:)', ...
         times(5,:)', times(6,:)', times(7,:)', times(8,:)',...
          times(9,:)', times(10,:)',...
          'VariableNames', {'AssistanceLevel', 't1', 't2', 't3', 't4', ...
          't5', 't6', 't7', 't8', 't9', 't10'});
      
    Time = [1 10]';
      
    rm = fitrm(T,'t1-t10~AssistanceLevel', 'WithinDesign', Time);
    
end