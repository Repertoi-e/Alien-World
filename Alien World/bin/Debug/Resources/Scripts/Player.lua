require "luanet"

function Player.Init()	
	InputManager.KeyPressedHandler:Add(function(event) 

			VirtualKey = luanet.import_type("WinApi.User32.VirtualKey")
			if event.KeyCode == VirtualKey.W then
				Log:InfoFormat("{0}, repeat: {1}", event.KeyCode, event.Repeat)
			end

			local pos = this.Entity:PositionComponent()
			pos.X = pos.X + 10
	end)
end

function Player.Update(dt)
	
end

function Player.Dispose()
	
end
