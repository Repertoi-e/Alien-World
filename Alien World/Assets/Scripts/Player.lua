local CLRPackage = require "CLRPackage"
local User32 = CLRPackage.Get("WinApi", "WinApi.User32")
local Alien_World = CLRPackage.Get("Alien_World", "")

local inputJump, inputLeft, inputRight = false, false, false
local _handler1, _handler2

function Player.Init()
	_handler1 = InputManager.KeyPressedHandler:Add(function(event) 
		if event.KeyCode == User32.VirtualKey.W then
			inputJump = true
		end
		if event.KeyCode == User32.VirtualKey.A then
			inputLeft = true
		end
		if event.KeyCode == User32.VirtualKey.D then
			inputRight = true
		end
	end)

	_handler2 = InputManager.KeyReleasedHandler:Add(function(event) 
		if event.KeyCode == User32.VirtualKey.W then
			inputJump = false
		end
		if event.KeyCode == User32.VirtualKey.A then
			inputLeft = false
		end
		if event.KeyCode == User32.VirtualKey.D then
			inputRight = false
		end
	end)
end

local airborne, running, flipped = false, false, false;
local toJump = 0;

-- Constants
local acc = 0.446875
local dec = 0.9
local frc = 0.446875
local top = 10
local topFall = 16
local gravity = acc * 2
local jmp = -16.5

-- Return the sign of a number (-1,1,0)
function sign(value)
	if value < 0 then return -1 end
	if value > 0 then return  1 end
	return 0
end

function Player.Update(dt)
		local velVec = this.Entity.velocity.Velocity
		local renderableInfo = this.Entity.renderable.Info
	
		running = velVec.X ~= 0.0;
		if toJump > 0 then 
			toJump = toJump - 1 
		end

		-- Jumping
		if inputJump and airborne == false and toJump == 0 then 
			velVec.Y = jmp
			airborne = true
		end

		if inputJump == false and airborne then 
			if velVec.Y < -10 then
				velVec.Y = -10
			end
		end

		-- Right and left movement
		if inputRight then
			if velVec.X < 0 then 
				velVec.X = velVec.X + dec 
			elseif velVec.X < top then 
				velVec.X = velVec.X + acc
			end
			renderableInfo.Flipped = false
		elseif inputLeft then
			if velVec.X > 0 then 
				velVec.X = velVec.X - dec 
			elseif velVec.X > -top then 
				velVec.X = velVec.X - acc
			end
			renderableInfo.Flipped = true
		else
			velVec.X = velVec.X - math.min(math.abs(velVec.X), frc)
				* sign(velVec.X)
		end

		-- While in air
		if airborne then
			renderableInfo.Reference = Alien_World.PlayerSprites.PlayerJump
			Alien_World.PlayerSprites.PlayerJump:SetFrame(math.abs(velVec.Y < 1 and 2 or (velVec.Y > 0 and 3 or 1)))
            if velVec.Y < 0 and velVec.Y > -4 then
                velVec.X = velVec.X - (math.floor(velVec.Y / 0.125) / 256)
			end
        else
			renderableInfo.Reference = running and Alien_World.PlayerSprites.PlayerRun or Alien_World.PlayerSprites.PlayerIdle
		end

		-- Gravity
		velVec.Y = velVec.Y + gravity;
        if velVec.Y > topFall then
            velVec.Y = topFall
		end

		-- Check if player has collided with ground
		local collisionResults = this.Entity.collision.LastCollisionResults
		if collisionResults ~= nil then
			for i=0,collisionResults.Count - 1 do
				local lastVector = collisionResults[i].TranslationVector
				if lastVector.Y < lastVector.X then
					airborne = false
				end
			end
		end

		this.Entity:ReplaceVelocity(velVec)
		this.Entity:ReplaceRenderable(renderableInfo)
end

function Player.Dispose()
	InputManager.KeyPressedHandler:Remove(_handler1)
	InputManager.KeyReleasedHandler:Remove(_handler2)
end
