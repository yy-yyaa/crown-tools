starter:
	xbuild /p:OutputPath=$(CROWN_INSTALL_DIR)/tools tools/starter/starter.sln
console:
	xbuild /p:OutputPath=$(CROWN_INSTALL_DIR)/tools tools/console/console.sln
tools: starter console
