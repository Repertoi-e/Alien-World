local CLRPackage = require "CLRPackage"
local User32 = CLRPackage.Get("WinApi", "WinApi.User32")
local Alien_World = CLRPackage.Get("Alien_World", "")

local inputJump, inputLeft, inputRight = false, false, false

function Player.Init()
	InputManager.KeyPressedHandler:Add(function(event) 
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

	InputManager.KeyReleasedHandler:Add(function(event) 
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

local acc = 0.446875
local dec = 0.9
local frc = 0.446875
local top = 10
local topFall = 16
local gravity = acc * 2
local jmp = -16.5

function sign(value)
	if value < 0 then return -1 end
	if value > 0 then return  1 end
	return 0
end

function Player.Update(dt)
		local velVec = this.Entity.velocity.Velocity
		local sprite = this.Entity.sprite
	
		running = velVec.X ~= 0.0;
		if toJump > 0 then 
			toJump = toJump - 1 
		end

		if inputJump and airborne == false and toJump == 0 then 
			velVec.Y = jmp
			airborne = true
		end

		if inputJump == false and airborne then 
			if velVec.Y < -10 then
				velVec.Y = -10
			end
		end

		if inputRight then
			if velVec.X < 0 then 
				velVec.X = velVec.X + dec 
			elseif velVec.X < top then 
				velVec.X = velVec.X + acc
			end
			sprite.Flipped = false
		elseif inputLeft then
			if velVec.X > 0 then 
				velVec.X = velVec.X - dec 
			elseif velVec.X > -top then 
				velVec.X = velVec.X - acc
			end
			sprite.Flipped = true
		else
			velVec.X = velVec.X - math.min(math.abs(velVec.X), frc)
				* sign(velVec.X)
		end

		if airborne then
			sprite.Renderable = Alien_World.PlayerSprites.PlayerJump
			Alien_World.PlayerSprites.PlayerJump:SetFrame(math.abs(velVec.Y < 1 and 2 or (velVec.Y > 0 and 3 or 1)))
            if velVec.Y < 0 and velVec.Y > -4 then
                velVec.X = velVec.X - (math.floor(velVec.Y / 0.125) / 256)
			end
        else
			sprite.Renderable = running and Alien_World.PlayerSprites.PlayerRun or Alien_World.PlayerSprites.PlayerIdle
		end

		velVec.Y = velVec.Y + gravity;
        if velVec.Y > topFall then
            velVec.Y = topFall
		end

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
		this.Entity:ReplaceSprite(sprite.Renderable, sprite.Flipped)
end

function Player.Dispose()

end
