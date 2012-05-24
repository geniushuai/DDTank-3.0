function Init()
    game:SetupMissions("1074");	
    game.TotalMissionCount = 3;
end

function Prepare()
	game.SessionId = 2;
end

function CalculateScoreGrade(rate)
	if rate > 800
	then
		return 3;
	elseif rate > 725 
	then
		return 2;
	elseif rate > 650
	then
		return 1;
	else
		return 0;
	end
end

function GameOverAllSession()

end
