SED=sed

ifeq ($(shell uname), Darwin)
	SED=gsed
endif

proto:
	rm -rf DemLock.Parser/proto
	mkdir -p ./DemLock.Parser/proto/tmp && \
		curl -L -o - https://github.com/SteamDatabase/Protobufs/archive/master.tar.gz | tar -xz --strip-components=1 -C ./DemLock.Parser/proto/tmp && \
		cp -a ./DemLock.Parser/proto/tmp/deadlock/* ./DemLock.Parser/proto/ && \
		cp -a ./DemLock.Parser/proto/tmp/google/ ./DemLock.Parser/proto/ && \
		rm -rf ./DemLock.Parser/proto/tmp
	rm -rf DemLock.Parser/proto/base_gcmessages.proto DemLock.Parser/proto/c_peer2peer_netmessages.proto DemLock.Parser/proto/citadel_clientmessages.proto DemLock.Parser/proto/citadel_gcmessages_client.proto DemLock.Parser/proto/citadel_gcmessages_server.proto DemLock.Parser/proto/citadel_usercmd.proto DemLock.Parser/proto/clientmessages.proto DemLock.Parser/proto/connectionless_netmessages.proto DemLock.Parser/proto/econ_gcmessages.proto DemLock.Parser/proto/econ_shared_enums.proto DemLock.Parser/proto/engine_gcmessages.proto DemLock.Parser/proto/enums_clientserver.proto DemLock.Parser/proto/gcsystemmsgs.proto DemLock.Parser/proto/network_connection.proto DemLock.Parser/proto/networksystem_protomessages.proto DemLock.Parser/proto/steamdatagram_messages_auth.proto DemLock.Parser/proto/steamdatagram_messages_sdr.proto DemLock.Parser/proto/steammessages_base.proto DemLock.Parser/proto/steammessages_cloud.steamworkssdk.proto DemLock.Parser/proto/steammessages_gamenetworkingui.proto DemLock.Parser/proto/steammessages_helprequest.steamworkssdk.proto DemLock.Parser/proto/steammessages_int.proto DemLock.Parser/proto/steammessages_oauth.steamworkssdk.proto DemLock.Parser/proto/steammessages_player.steamworkssdk.proto DemLock.Parser/proto/steammessages_publishedfile.steamworkssdk.proto DemLock.Parser/proto/steamnetworkingsockets_messages.proto DemLock.Parser/proto/steamnetworkingsockets_messages_certs.proto DemLock.Parser/proto/steamnetworkingsockets_messages_udp.proto DemLock.Parser/proto/uifontfile_format.proto DemLock.Parser/proto/usercmd.proto DemLock.Parser/proto/google/protobuf/any.proto DemLock.Parser/proto/google/protobuf/source_context.proto DemLock.Parser/proto/google/protobuf/type.proto DemLock.Parser/proto/google/protobuf/wrappers.proto
	$(SED) -i '/^import "network_connection\.proto"/d' DemLock.Parser/proto/networkbasetypes.proto
	$(SED) -i '/^import "google\/protobuf\/descriptor\.proto"/d' DemLock.Parser/proto/citadel_gameevents.proto
	$(SED) -i '/^import "citadel_gcmessages_common.proto"/d' DemLock.Parser/proto/citadel_gamemessages.proto
	protoc -I DemLock.Parser/proto --csharp_out=DemLock.Parser/proto DemLock.Parser/proto/*.proto
