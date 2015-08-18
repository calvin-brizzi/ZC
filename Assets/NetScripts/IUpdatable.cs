using System;

public interface IUpdatable {
	
	void Turn(int gameFramesPerSecond);
	
	bool Finished { get; }
}

