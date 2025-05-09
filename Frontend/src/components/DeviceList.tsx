import { useEffect, useState } from 'react';
import { Alert, Box, CircularProgress, IconButton, List, Typography } from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import { DevicesResponse } from '@/types/vreedaApi';
import DeviceControl from './DeviceControl';

export default function DeviceList({selectedDevices, onSelectionChange}: {selectedDevices: string[], onSelectionChange: (id: string, selected: boolean) => void }) {
  const [devices, setDevices] = useState<DevicesResponse>({});
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [reloading, setReloading] = useState<boolean>(false);

  const fetchDevices = async () => {
    setError(null);
    try {
      const response = await fetch('/api/vreeda/list-devices');
      if (!response.ok) throw new Error('Failed to fetch devices');
      const result = await response.json();
      if (!result.success) {
        throw new Error(result.error || 'Failed to fetch devices');
      }
      setDevices(result.data as DevicesResponse);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
      setReloading(false); // Reset reloading state
    }
  };

  useEffect(() => {
    fetchDevices();
  }, []);

  // Handler for the reload button
  const handleReload = () => {
    setReloading(true);
    fetchDevices();
  };

  return (
    <Box sx={{ width: '100%', pt: 4 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Typography variant="h5" gutterBottom>
          Devices
        </Typography>
        <IconButton onClick={handleReload} disabled={reloading}>
          {reloading ? <CircularProgress size={24} /> : <RefreshIcon />}
        </IconButton>
      </Box>
      {error && <Alert severity="error">Error: {error}</Alert>}
      {loading ? (
        <Box display="flex" justifyContent="center" pt={2}>
          <CircularProgress />
        </Box>
      ) : (
        <List>
          {devices &&
            Object.entries(devices).map(([id, device]) => (
                <DeviceControl model={device} id={id} key={id} selected={selectedDevices.includes(id)} onSelectionChange={onSelectionChange}/>
            ))}
        </List>
      )}
    </Box>
  );
}